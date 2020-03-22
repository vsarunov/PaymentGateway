using LanguageExt;
using System;

namespace PaymentGateway.Domain
{
    public class Card : Record<Card>
    {
        public Card(Guid userId, string number, int cVV, ExpirationDate expiration)
        {
            Number = number ?? throw new ArgumentNullException(nameof(number));
            Expiration = expiration ?? throw new ArgumentNullException(nameof(Expiration));
            CVV = cVV;
            UserId = userId;
        }

        public Card(long id, Guid userId, string number, int cVV, ExpirationDate expiration)
        {
            Id = id;
            Number = number ?? throw new ArgumentNullException(nameof(number));
            Expiration = expiration ?? throw new ArgumentNullException(nameof(Expiration));
            CVV = cVV;
            UserId = userId;
        }

        public Guid UserId { get; }
        public long Id { get; }
        public string Number { get; }
        public int CVV { get; }
        public ExpirationDate Expiration { get; }
    }
}
