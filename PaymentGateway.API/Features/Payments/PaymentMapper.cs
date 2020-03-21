using PaymentGateway.Application.Payments.Commands;
using PaymentGateway.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway.API.Features.Payments
{
    public static class PaymentMapper
    {
        internal static CreatePayment ToCommand(this CreatePaymentRequest request, Guid userId)
        {
            return new CreatePayment
            {
                CardDetails = new CreatePayment.Card
                {
                    CVV = request.CardDetails.CVV,
                    Number = request.CardDetails.Number,
                    Expiration = new CreatePayment.ExpirationDate
                    {
                        Month = request.CardDetails.Expiration.Month,
                        Year = request.CardDetails.Expiration.Year,
                    }
                },
                Value = new CreatePayment.Money
                {
                    Amount = request.Value.Amount,
                    Currency = request.Value.ISOCurrencyCode
                },
                TimeStamp = request.TimeStamp,
                UserId = userId
            };
        }

        internal static IEnumerable<GetPaymentsResponse> ToResponse(this IEnumerable<Payment> entities)
        {
            return entities.Select(x => new GetPaymentsResponse());
        }
    }
}
