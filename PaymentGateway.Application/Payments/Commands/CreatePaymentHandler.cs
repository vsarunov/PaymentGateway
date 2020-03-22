using LanguageExt;
using LanguageExt.Common;
using MediatR;
using PaymentGateway.Application.Cards.Queries;
using PaymentGateway.Application.Common;
using PaymentGateway.Application.Extensions;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace PaymentGateway.Application.Payments.Commands
{
    public class CreatePaymentHandler : IRequestHandler<CreatePayment, Either<Seq<Failure>, int>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMediator _mediator;
        public CreatePaymentHandler(IMediator mediator, IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<Either<Seq<Failure>, int>> Handle(CreatePayment request, CancellationToken cancellationToken)
        {
            var paymentValidation = await ValidatePayment(request);

            return await (await paymentValidation
                .MatchAsync(
                    ValidateWithBank(request),
                    left => left
                  ))
                .ToEitherAsync();
        }

        private Func<CreatePayment, Task<Validation<Failure, Task<int>>>> ValidateWithBank(CreatePayment request) =>
            async right =>
                {
                    var bankValidation = await GetBankValidation(request);
                    var payment = request.ToPayment();
                    return (await bankValidation.MatchAsync<Validation<Error, Payment>>(
                        async r =>
                        {
                            payment.UpdateStatus(PaymentStatus.Success);
                            return payment;
                        },
                        async l =>
                        {
                            await CreateAsync(payment);
                            return l;
                        })
                      .Apply(p => p))
                      .MapFail(Failure.Of(request))
                      .Map(CreateAsync);
                };


        private Task<int> CreateAsync(Payment payment) => _paymentRepository.SavePaymentAsync(payment);

        private async Task<Validation<Failure, CreatePayment>> ValidatePayment(CreatePayment command) =>
            (await ValidateIfPaymentAlreadyExists(command), await ValidateIfCardExistWithDifferentUser(command))
                .Apply((paymentValidation, cardValidation) => command)
                .MapFail(Failure.Of(command));


        private async Task<Validation<Error, CreatePayment>> ValidateIfPaymentAlreadyExists(CreatePayment command)
        {
            var payments = await _paymentRepository.GetPaymentsByUserIdAsync(command.UserId);
            return payments.Any(p => IsMatchingPayment(command, p))
               ? Fail<Error, CreatePayment>(Error.New($"Payment already exists:{command}"))
               : Success<Error, CreatePayment>(command);
        }

        private bool IsMatchingPayment(CreatePayment command, Payment p)
        {
            return p.Card.CVV == command.CardDetails.CVV
                            && p.Card.Number == command.CardDetails.Number
                            && p.TimeStamp == command.TimeStamp
                            && p.Value.Amount == command.Value.Amount
                            && p.Value.ISOCurrencyCode == command.Value.Currency;
        }

        private async Task<Validation<Error, CreatePayment>> GetBankValidation(CreatePayment command) =>
            (await _mediator.Send(command.ToBankVerificationQuery()))
                 ? Success<Error, CreatePayment>(command)
                 : Fail<Error, CreatePayment>(Error.New($"Payment has not been verified:{command}"));

        private async Task<Validation<Error, CreatePayment>> ValidateIfCardExistWithDifferentUser(CreatePayment command)
        {
            return (await _mediator.Send(new GetCardByNumber(command.CardDetails.Number)))
                .Map(c => c.UserId)
                .Exists(c => c != command.UserId)
                 ? Fail<Error, CreatePayment>(Error.New($"Card is in use"))
                 : Success<Error, CreatePayment>(command);
        }
    }
}
