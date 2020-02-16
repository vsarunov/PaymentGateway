using PaymentGateway.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway.Infrastructure
{
    public class PaymentRepository : IPaymentRepository
    {
        public Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> SavePaymentAsync(Payment payment)
        {
            throw new NotImplementedException();
        }
    }
}
