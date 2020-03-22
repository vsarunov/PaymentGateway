using MediatR;
using PaymentGateway.Application.Common;
using System;

namespace PaymentGateway.Application.Banks.Queries
{
    public class GetBankPaymentVerification : IRequest<bool>
    {
        public DateTime TimeStamp { get; }
        public Card CardDetails { get; }
        public Money Value { get; }

        public GetBankPaymentVerification(DateTime timeStamp, Money value, Card card)
        {
            TimeStamp = timeStamp;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            CardDetails = card ?? throw new ArgumentNullException(nameof(card));
        }
    }
}
