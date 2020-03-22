namespace PaymentGateway.API.Features.Payments
{
    public class GetPaymentsResponse
    {
        public long Id { get; set; }
        public string CardNumber { get; set; }
        public string PaymentStatus { get; set; }
        public MoneyResponse Value { get; set; }

        public class MoneyResponse
        {
            public decimal Amount { get; set; }
            public string ISOCurrencyCode { get; set; }
        }
    }
}
