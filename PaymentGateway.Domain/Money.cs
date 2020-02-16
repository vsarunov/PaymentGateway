using LanguageExt;
using System;

namespace PaymentGateway.Domain  
{
    public class Money: Record<Money>
    {
        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        }

        public decimal Amount { get; }
        public string Currency { get; }
    }
}
