using LanguageExt;
using System;

namespace PaymentGateway.Domain
{
    public class Money : Record<Money>
    {
        public Money(decimal amount, string currency)
        {
            Amount = amount;
            ISOCurrencyCode = currency ?? throw new ArgumentNullException(nameof(currency));
        }

        public decimal Amount { get; }
        public string ISOCurrencyCode { get; }
    }
}
