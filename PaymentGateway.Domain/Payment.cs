using LanguageExt;
using PaymentGateway.Domain.Enums;
using System;

namespace PaymentGateway.Domain
{
    public class Payment : Record<Payment>
    {
        public long Id { get; }
        public Guid UserId { get; }
        public Card Card { get; }
        public Money Value { get; }
        public DateTime TimeStamp { get; }
        public PaymentStatus Status { get; protected set; } = PaymentStatus.Fail;

        public Payment(long id, Guid userId, Card cardDetails, Money value, DateTime timeStamp, PaymentStatus status)
        {
            Id = id;
            UserId = userId;
            Card = cardDetails ?? throw new ArgumentNullException(nameof(cardDetails));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            TimeStamp = timeStamp;
        }

        public Payment(Guid userId, Card card, Money value, DateTime timeStamp)
        {
            UserId = userId;
            Card = card ?? throw new ArgumentNullException(nameof(card));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            TimeStamp = timeStamp;
        }

        public void UpdateStatus(PaymentStatus status)
        {
            Status = status;
        }

        public override string ToString()
        {
            return $"Payment{Id},{Value.Amount}.{Value.ISOCurrencyCode},{TimeStamp},{Status}";
        }
    }
}
