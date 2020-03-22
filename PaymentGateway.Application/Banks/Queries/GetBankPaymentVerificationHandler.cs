using MediatR;
using PaymentGateway.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Banks.Queries
{
    public class GetBankPaymentVerificationHandler : IRequestHandler<GetBankPaymentVerification, bool>
    {
        private readonly IBankRepository _bankRepository;
        public GetBankPaymentVerificationHandler(IBankRepository bankRepository)
        {
            _bankRepository = bankRepository ?? throw new ArgumentNullException(nameof(bankRepository));
        }

        public Task<bool> Handle(GetBankPaymentVerification request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
