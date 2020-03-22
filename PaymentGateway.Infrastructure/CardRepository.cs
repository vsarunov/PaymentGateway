using LanguageExt;
using PaymentGateway.Domain;
using System.Threading.Tasks;

namespace PaymentGateway.Infrastructure
{
    public class CardRepository : ICardRepository
    {
        public Task<Option<Card>> GetCardByNumberAsync(string cardNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}
