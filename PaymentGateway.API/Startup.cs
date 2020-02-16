using Hellang.Middleware.ProblemDetails;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.API.Extensions;
using PaymentGateway.Application.Payments.Commands;

namespace PaymentGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddOpenApi()
                .AddProblemDetails()
                .AddAuthentication(Configuration)
                .AddMediatR(typeof(CreatePayment.Handler).Assembly)
                .AddRepositoryDependencies()
                .AddMvc(options => options.AddAuthorizationFilter(Configuration))
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                    .UseOpenApiUI();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection()
               .UseAuthentication()
               .UseMvc()
               .UseProblemDetails();
        }
    }
}
