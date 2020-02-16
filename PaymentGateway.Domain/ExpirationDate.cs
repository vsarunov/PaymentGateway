using LanguageExt;

namespace PaymentGateway.Domain
{
    public class ExpirationDate : Record<ExpirationDate>
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
