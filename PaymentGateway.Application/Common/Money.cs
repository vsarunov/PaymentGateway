using System;

namespace PaymentGateway.Application.Common
{
    public class Money
    {
        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = string.IsNullOrEmpty(currency) ? throw new ArgumentNullException(nameof(currency)) : currency;
        }

        public decimal Amount { get; }
        public string Currency { get; }
    }
}
