using LanguageExt;
using MediatR;
using PaymentGateway.Domain;
using System;

namespace PaymentGateway.Application.Cards.Queries
{
    public class GetCardByNumber : IRequest<Option<Card>>
    {
        public GetCardByNumber(string cardNumber)
        {
            CardNumber = cardNumber ?? throw new ArgumentNullException(nameof(cardNumber));
        }
        public string CardNumber { get; }
    }
}
