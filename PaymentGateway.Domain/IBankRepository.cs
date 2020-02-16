using System.Threading.Tasks;

namespace PaymentGateway.Domain
{
    public interface IBankRepository
    {
        Task<bool> IsValidPayment(Payment payment);
    }
}
