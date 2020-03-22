using System;

namespace PaymentGateway.Application.Common
{
    public class Card
    {
        public Card(string number, int cvv, ExpirationDate expiration)
        {
            Number = string.IsNullOrEmpty(number) ? throw new ArgumentNullException(nameof(number)) : number;
            Expiration = expiration ?? throw new ArgumentNullException(nameof(number));
            CVV = cvv;
        }

        public string Number { get; }
        public int CVV { get; }
        public ExpirationDate Expiration { get; }
    }
}
