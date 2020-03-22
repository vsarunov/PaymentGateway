using PaymentGateway.Domain;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.Infrastructure
{
    public class BankRepository : IBankRepository
    {
        public async Task<bool> IsValidPayment(Payment payment)
        {
            throw new NotImplementedException();
        }
    }
}
