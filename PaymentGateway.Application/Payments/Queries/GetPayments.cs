using MediatR;
using PaymentGateway.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Payments.Queries
{
    public class GetPayments
    {
        public class Query : IRequest<IEnumerable<Payment>>
        {

        }

        public class Handler : IRequestHandler<Query, IEnumerable<Payment>>
        {
            public Task<IEnumerable<Payment>> Handle(Query request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
