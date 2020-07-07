using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Client.Payments.Api.Api.Middleware;
using Client.Payments.Api.Domain;
using Client.Payments.Api.Infrastructure;
using Client.Payments.Api.Infrastructure.PaymentGateway;
using Client.Payments.Api.Infrastructure.PaymentGatewayProcessor;
using Client.Payments.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
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

            
            services.AddAuthentication(cfg =>
                {
                    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    cfg.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer( options =>
                {

                    options.Authority =  Configuration["IdentityServer"];
                    options.Audience = "amazonId";
                   
                });
            // create a named HttpClient that bypasses that allows untrusted certificates
            services.AddHttpClient("HttpClientWithSSLUntrusted")
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) => true
            });
          
            services.AddTransient<ITokenProvider, TokenProvider>();
            services.AddTransient<IPaymentGatewayProcessorProxy, PaymentGatewayProcessorProxy>();
            services.AddTransient<IPaymentGatewayProxy,PaymentGatewayProxy>();
          

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
            // Use the Prometheus middleware
            app.UseMetricServer();
            app.UseHttpMetrics();

            loggerFactory.AddSerilog();
            app.UseMiddleware(typeof(ExceptionMiddleware));
            app.UseRouting();

            // Use the Prometheus middleware
            app.UseMetricServer();
            app.UseHttpMetrics();

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
