using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.API.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class OpenApiExtensions
    {
        private static OpenApiSecurityScheme SecurityDefinition => new OpenApiSecurityScheme
        {
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Please enter JWT (including Bearer) into the field, ex. \"Bearer {token}\"",
            Name = "Authorization",
            Type = OpenApiSecuritySchemeType.ApiKey
        };

        public static IServiceCollection AddOpenApi(this IServiceCollection services)
        {
            return services.AddOpenApiDocument(options =>
            {
                options.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Payment Gateway";
                };
                options.AddSecurity("Bearer", Enumerable.Empty<string>(), SecurityDefinition);
            });
        }

        public static IApplicationBuilder UseOpenApiUI(this IApplicationBuilder app)
        {
            return app.UseOpenApi().UseSwaggerUi3();
        }
    }
}
