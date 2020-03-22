using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Domain;
using PaymentGateway.Infrastructure;

namespace PaymentGateway.API.Extensions
{
    public static class DependenciesExtension
    {

        public static IServiceCollection AddRepositoryDependencies(this IServiceCollection services)
        {
            return services
                .AddScoped<IPaymentRepository, PaymentRepository>()
                .AddScoped<IBankRepository, BankRepository>()
                .AddScoped<ICardRepository, CardRepository>();
        }
    }
}
