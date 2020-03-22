using LanguageExt;
using System.Threading.Tasks;

namespace PaymentGateway.Domain
{
    public interface ICardRepository
    {
        Task<Option<Card>> GetCardByNumberAsync(string cardNumber);
    }
}
