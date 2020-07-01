using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Client.Payments.Api.Domain;
using Client.Payments.Api.Infrastructure;
using Client.Payments.Api.Infrastructure.PaymentGateway;
using Client.Payments.Api.Infrastructure.PaymentGatewayProcessor;
using Client.Payments.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client.Payments.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<IPaymentGatewayProcessorProxy, PaymentGatewayProcessorProxy>(client =>
                {
                    var paymentGatewayProcessorAddress = Configuration["PaymentGatewayProcessorAddress"];
                    client.BaseAddress = new Uri(paymentGatewayProcessorAddress);

                    client.DefaultRequestHeaders.Accept.Clear();

                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                }
            );
            services.AddHttpClient<IPaymentGatewayProxy, PaymentGatewayProxy>(client =>
                {
                    var paymentGatewayAddress = Configuration["PaymentGatewayAddress"];
                    client.BaseAddress = new Uri(paymentGatewayAddress);

                    client.DefaultRequestHeaders.Accept.Clear();

                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                }
            );

            services.AddTransient<IPaymentService, PaymentService>();
            services.AddControllers();
            services.AddHealthChecks();
            services.AddAutoMapper(typeof(Startup));
        }

     
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
