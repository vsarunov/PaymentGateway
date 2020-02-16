using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Common.Configurations;

namespace PaymentGateway.API.Extensions
{
    public static class AuthenticationExtensions
    {
        public static MvcOptions AddAuthorizationFilter(this MvcOptions options, IConfiguration configuration)
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            options?.Filters.Add(new AuthorizeFilter(policy));

            return options;
        }

        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    var jwtConfiguration = GetConfiguration(configuration);

                    options.Authority = jwtConfiguration.Authority;
                    options.RequireHttpsMetadata = jwtConfiguration.RequireHttpsMetadata;
                    options.ApiName = jwtConfiguration.ApiName;
                    options.ApiSecret = jwtConfiguration.ApiSecret;
                });

            return services;
        }

        private static Authentication GetConfiguration(this IConfiguration configuration)
        {
            var authentication = new Authentication();
            configuration.Bind("Authentication", authentication);

            return authentication;
        }
    }
}
