using MediatR;
using PaymentGateway.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Payments.Queries
{
    public class GetPayments
    {
        public class Query : IRequest<IEnumerable<Payment>>
        {
            public Query(Guid userId)
            {
                UserId = userId;
            }

            public Guid UserId { get; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<Payment>>
        {
            private readonly IPaymentRepository _paymentRepository;
            public Handler(IPaymentRepository paymentRepository)
            {
                _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            }

            public Task<IEnumerable<Payment>> Handle(Query request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
