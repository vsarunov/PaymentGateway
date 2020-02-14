using PaymentGateway.Application.Payments.Commands;
using PaymentGateway.Domain;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway.API.Features.Payments
{
    public static class PaymentMapper
    {
        internal static CreatePayment.Command ToCommand(this CreatePaymentRequest request)
        {
            return new CreatePayment.Command
            {
                CardDetails = new CreatePayment.Command.Card
                {
                    CVV = request.CardDetails.CVV,
                    Number = request.CardDetails.Number,
                    Expiration = new CreatePayment.Command.ExpirationDate
                    {
                        Month = request.CardDetails.Expiration.Month,
                        Year = request.CardDetails.Expiration.Year,
                    }
                },
                Value = new CreatePayment.Command.Money
                {
                    Amount = request.Value.Amount,
                    Currency = request.Value.ISOCurrencyCode
                }
            };
        }

        internal static IEnumerable<GetPaymentsResponse> ToResponse(this IEnumerable<Payment> entities)
        {
            return entities.Select(x => new GetPaymentsResponse());
        }
    }
}
