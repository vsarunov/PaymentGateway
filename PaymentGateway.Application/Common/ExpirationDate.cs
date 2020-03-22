namespace PaymentGateway.Application.Common
{
    public class ExpirationDate
    {
        public ExpirationDate(int year, int month)
        {
            Year = year;
            Month = month;
        }

        public int Year { get; }
        public int Month { get; }
    }
}
