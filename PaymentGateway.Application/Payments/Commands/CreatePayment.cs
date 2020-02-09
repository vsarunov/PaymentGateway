using LanguageExt;
using LanguageExt.Common;
using MediatR;
using PaymentGateway.Application.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Payments.Commands
{
    public class CreatePayment
    {
        public class Command : IRequest<Either<Seq<Failure>, int>>
        {
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
                public short CVV { get; set; }
                public ExpirationDate Expiration { get; set; }
            }

            public class ExpirationDate
            {
                public short Year { get; set; }
                public byte Month { get; set; }
            }
        }

        public class Handler : IRequestHandler<Command, Either<Seq<Failure>, int>>
        {
            public Task<Either<Seq<Failure>, int>> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
