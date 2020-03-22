using MediatR;
using PaymentGateway.Domain;
using System;
using System.Collections.Generic;

namespace PaymentGateway.Application.Payments.Queries
{
    public class GetPaymentsByUserId : IRequest<IEnumerable<Payment>>
    {
        public GetPaymentsByUserId(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }
    }

}
