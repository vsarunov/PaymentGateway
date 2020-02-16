using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway.Domain
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(Guid userId);
        Task<int> SavePaymentAsync(Payment payment);
    }
}
