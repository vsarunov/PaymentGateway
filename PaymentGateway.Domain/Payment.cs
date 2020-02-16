using System;

namespace PaymentGateway.Domain
{
    public class Payment
    {
        public long Id { get; }
        public Guid UserId { get; }
        public Card CardDetails { get; }
        public Money Value { get; }
        public DateTime TimeStamp { get; }

        public Payment(long id, Guid userId, Card cardDetails, Money value, DateTime timeStamp)
        {
            Id = id;
            UserId = userId;
            CardDetails = cardDetails ?? throw new ArgumentNullException(nameof(cardDetails));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            TimeStamp = timeStamp;
        }

        public Payment(Guid userId, Card cardDetails, Money value, DateTime timeStamp)
        {
            UserId = userId;
            CardDetails = cardDetails ?? throw new ArgumentNullException(nameof(cardDetails));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            TimeStamp = timeStamp;
        }

        public override string ToString()
        {
            return $"Payment{Id},{Value.Amount}.{Value.Currency},{TimeStamp}";
        }
    }
}
