using PaymentGateway.Application.Banks.Queries;
using PaymentGateway.Domain;

namespace PaymentGateway.Application.Payments.Commands
{
    internal static class CreatePaymentCommandMapper
    {
        internal static Payment ToPayment(this CreatePayment command) =>
            new Payment
                (
                command.UserId,
                new Card
                    (
                    command.UserId,
                    command.CardDetails.Number,
                    command.CardDetails.CVV,
                    new ExpirationDate(command.CardDetails.Expiration.Year, command.CardDetails.Expiration.Month)
                    ),
                new Money(command.Value.Amount, command.Value.Currency),
                command.TimeStamp
                );
        

        internal static GetBankPaymentVerification ToBankVerificationQuery(this CreatePayment payment) =>
            new GetBankPaymentVerification
                (
                payment.TimeStamp,
                new Common.Money(payment.Value.Amount, payment.Value.Currency),
                new Common.Card
                    (
                    payment.CardDetails.Number,
                    payment.CardDetails.CVV,
                    new Common.ExpirationDate(payment.CardDetails.Expiration.Year, payment.CardDetails.Expiration.Year)
                ));
    }
}
