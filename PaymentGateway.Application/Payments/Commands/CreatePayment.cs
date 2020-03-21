using LanguageExt;
using MediatR;
using PaymentGateway.Application.Common;
using System;

namespace PaymentGateway.Application.Payments.Commands
{
    public class CreatePayment : IRequest<Either<Seq<Failure>, int>>
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
}
