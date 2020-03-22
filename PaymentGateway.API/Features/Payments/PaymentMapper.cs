using PaymentGateway.Application.Payments.Commands;
using Payment = PaymentGateway.Domain.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using PaymentGateway.Application.Common;

namespace PaymentGateway.API.Features.Payments
{
    public static class PaymentMapper
    {
        internal static CreatePayment ToCommand(this CreatePaymentRequest request, Guid userId)
        {
            return new CreatePayment
                (
                userId,
                request.TimeStamp,
                new Money(request.Value.Amount, request.Value.ISOCurrencyCode),
                new Card
                    (
                    request.CardDetails.Number,
                    request.CardDetails.CVV,
                    new ExpirationDate
                        (
                        request.CardDetails.Expiration.Year,
                        request.CardDetails.Expiration.Month
                        )
                    )
                );
        }

        internal static IEnumerable<GetPaymentsResponse> ToResponse(this IEnumerable<Payment> entities)
        {
            return entities.Select(x => new GetPaymentsResponse
            {
                Id = x.Id,
                PaymentStatus = x.Status.ToString(),
                CardNumber = new string(x.Card?.Number?.Select((p, index) => index <= 12 ? '*' : p).ToArray()),
                Value = new GetPaymentsResponse.MoneyResponse
                {
                    Amount = x.Value.Amount,
                    ISOCurrencyCode = x.Value.ISOCurrencyCode
                }
            });
        }
    }
}
