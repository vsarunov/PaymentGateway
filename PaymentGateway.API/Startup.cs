﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hellang.Middleware.ProblemDetails;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddOpenApi()
                .AddProblemDetails()
                .AddMediatR(typeof(CreatePayment.Handler).Assembly)           
                .AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
               .UseMvc()
               .UseProblemDetails();
        }
    }
}
