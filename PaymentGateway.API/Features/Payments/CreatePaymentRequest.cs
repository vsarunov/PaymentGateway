using System;
using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.API.Features.Payments
{
    public class CreatePaymentRequest
    {
        public CardDto CardDetails { get; set; }
        public MoneyDto Value { get; set; }
        public DateTime TimeStamp { get; set; }

        public class MoneyDto
        {
            public decimal Amount { get; set; }
            public string Currency { get; set; }
        }

        public class CardDto
        {
            public string Number { get; set; }
            public short CVV { get; set; }
            public ExpirationDateDto Expiration { get; set; }
        }

        public class ExpirationDateDto
        {
            public int Year { get; set; }
            public string Month { get; set; }
        }
    }
}
