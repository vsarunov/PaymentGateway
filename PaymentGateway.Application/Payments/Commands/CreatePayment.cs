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

        public CreatePayment(Guid userId, DateTime timeStamp, Money value, Card card)
        {
            UserId = userId;
            TimeStamp = timeStamp;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            CardDetails = card ?? throw new ArgumentNullException(nameof(card));
        }
    }
}
