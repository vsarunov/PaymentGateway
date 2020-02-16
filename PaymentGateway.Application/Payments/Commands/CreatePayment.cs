using LanguageExt;
using LanguageExt.Common;
using MediatR;
using PaymentGateway.Application.Common;
using PaymentGateway.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Payments.Commands
{
    public class CreatePayment
    {
        public class Command : IRequest<Either<Seq<Failure>, int>>
        {
            public Guid UserId { get; set; }
            public DateTime TimeStamp { get; set; }
            public Card CardDetails { get; set; }
            public Money Value { get; set; }

            public class Money
            {
                public decimal Amount { get; set; }
                public string Currency { get; set; }
            }

            public class Card
            {
                public string Number { get; set; }
                public int CVV { get; set; }
                public ExpirationDate Expiration { get; set; }
            }

            public class ExpirationDate
            {
                public int Year { get; set; }
                public int Month { get; set; }
            }
        }

        public class Handler : IRequestHandler<Command, Either<Seq<Failure>, int>>
        {
            private readonly IPaymentRepository _paymentRepository;
            private readonly IBankRepository _bankRepository;
            public Handler(IPaymentRepository paymentRepository, IBankRepository bankRepository)
            {
                _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
                _bankRepository = bankRepository ?? throw new ArgumentNullException(nameof(bankRepository));
            }
            public Task<Either<Seq<Failure>, int>> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
