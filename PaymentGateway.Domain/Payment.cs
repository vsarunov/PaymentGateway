using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.Domain
{
    public class Payment
    {
        public Card CardDetails { get; set; }
        public Money Value { get; set; }
        public DateTime TimeStamp { get; set; }

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
            public int Year { get; set; }
            public int Month { get; set; }
        }
    }
}
