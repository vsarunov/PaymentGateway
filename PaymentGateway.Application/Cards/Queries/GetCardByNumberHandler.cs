using LanguageExt;
using MediatR;
using PaymentGateway.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Cards.Queries
{
    public class GetCardByNumberHandler : IRequestHandler<GetCardByNumber, Option<Card>>
    {
        private readonly ICardRepository _cardRepository;

        public GetCardByNumberHandler(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository ?? throw new ArgumentNullException(nameof(cardRepository));
        }

        public async Task<Option<Card>> Handle(GetCardByNumber request, CancellationToken cancellationToken)
        {
            return await _cardRepository.GetCardByNumberAsync(request.CardNumber);
        }
    }
}
