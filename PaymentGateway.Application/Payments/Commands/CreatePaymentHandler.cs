using LanguageExt;
using MediatR;
using PaymentGateway.Application.Common;
using PaymentGateway.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Payments.Commands
{
    public class CreatePaymentHandler : IRequestHandler<CreatePayment, Either<Seq<Failure>, int>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBankRepository _bankRepository;
        public CreatePaymentHandler(IPaymentRepository paymentRepository, IBankRepository bankRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _bankRepository = bankRepository ?? throw new ArgumentNullException(nameof(bankRepository));
        }
        public Task<Either<Seq<Failure>, int>> Handle(CreatePayment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
