using LanguageExt;
using System;

namespace PaymentGateway.Domain
{
    public class Card: Record<Card>
    {
        public Card(string number, short cVV, ExpirationDate expiration)
        {
            Number = number ?? throw new ArgumentNullException(nameof(number));
            Expiration = expiration ?? throw new ArgumentNullException(nameof(Expiration));
            CVV = cVV; 
        }

        public string Number { get; }
        public short CVV { get; }
        public ExpirationDate Expiration { get; }
    }
}
