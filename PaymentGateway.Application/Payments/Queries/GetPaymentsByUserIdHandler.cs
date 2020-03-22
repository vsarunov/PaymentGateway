using MediatR;
using PaymentGateway.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Payments.Queries
{
    public class GetPaymentsByUserIdHandler : IRequestHandler<GetPaymentsByUserId, IEnumerable<Payment>>
    {
        private readonly IPaymentRepository _paymentRepository;
        public GetPaymentsByUserIdHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public async Task<IEnumerable<Payment>> Handle(GetPaymentsByUserId request, CancellationToken cancellationToken)
        {
            return await _paymentRepository.GetPaymentsByUserIdAsync(request.UserId);
        }
    }
}
