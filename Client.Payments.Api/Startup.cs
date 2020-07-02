using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Client.Payments.Api.Api.Middleware;
using Client.Payments.Api.Domain;
using Client.Payments.Api.Infrastructure;
using Client.Payments.Api.Infrastructure.PaymentGateway;
using Client.Payments.Api.Infrastructure.PaymentGatewayProcessor;
using Client.Payments.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Serilog;

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

            
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", config =>
                {
                    config.Authority =  Configuration["IdentityServer"];
                    config.Audience = "amazonId";
                    //config.RequireHttpsMetadata = false;
                });
            services.AddHttpClient();
            services.AddTransient<ITokenProvider, TokenProvider>();

            services.AddHttpClient<IPaymentGatewayProcessorProxy, PaymentGatewayProcessorProxy>(
            client =>
                {
                    var paymentGatewayProcessorAddress = Configuration["PaymentGatewayProcessorAddress"];
                    client.BaseAddress = new Uri(paymentGatewayProcessorAddress);

                    client.DefaultRequestHeaders.Accept.Clear();

                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                }
            );
            services.AddTransient<IPaymentGatewayProxy,PaymentGatewayProxy>();
            //client =>
            //    {
            //        var paymentGatewayAddress = Configuration["PaymentGatewayAddress"];
            //        client.BaseAddress = new Uri(paymentGatewayAddress);

            //        client.DefaultRequestHeaders.Accept.Clear();

            //        client.DefaultRequestHeaders.Accept.Add(
            //            new MediaTypeWithQualityHeaderValue("application/json"));
            //    }
            //);

            services.AddTransient<IPaymentService, PaymentService>();
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Client Api Demo", Version = "v1" });
            });
            services.AddHealthChecks();

            services.AddAutoMapper(typeof(Startup));
        }

     
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddSerilog();
            app.UseMiddleware(typeof(ExceptionMiddleware));
            app.UseRouting();

            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false
            };

            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();

            // ref: https://github.com/aspnet/Docs/issues/2384
            app.UseForwardedHeaders(forwardOptions);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Client Api Demo V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
