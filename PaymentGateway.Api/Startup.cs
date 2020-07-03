using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PaymentGateway.Api.Attributes;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Infrastructure;
using PaymentGateway.Api.Middleware;
using PaymentGateway.Api.Services;
using PaymentGateway.SharedLib.Encryption;
using PaymentGateway.SharedLib.EventBroker;
using RabbitMQ.Client;
using Serilog;
using Swashbuckle;
namespace PaymentGateway.Api
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAuth(services);

            // Add entity entity framework .
            var sqlConnectionString = Configuration.GetConnectionString("SqlConnection");
            services.AddDbContext<PaymentGatewayDbContext>(options => options.UseSqlServer(sqlConnectionString));
            services.AddScoped<IMerchantRepository, EfMerchantRepository>();
            services.AddScoped<IPaymentRepository, EfPaymentRepository>();

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICipherService, AesCipherService>();
            services.AddTransient<IErrorMapper,ErrorMapper>();
            services.AddControllers();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PaymentGateway Demo", Version = "v1" });
            });
            //services.AddDbContext<PaymentGatewayDbContext>(opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()),ServiceLifetime.Scoped,ServiceLifetime.Scoped);
            services.AddHealthChecks();
            services.AddAutoMapper(typeof(Startup));

            //---------------------------------------
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBrokerConnection"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(Configuration["EventBrokerUserName"]))
                {
                    factory.UserName = Configuration["EventBrokerUserName"];
                }

                if (!string.IsNullOrEmpty(Configuration["EventBrokerPassword"]))
                {
                    factory.Password = Configuration["EventBrokerPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBrokerRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBrokerRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            RegisterEventPusPublisher(services);
        }

       
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddSerilog();

            app.UseMiddleware(typeof(ExceptionMiddleware));
            app.UseMiddleware(typeof(RequestIdLoggingMiddleware));

            app.UseRouting();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentGateway Demo V1");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapDefaultControllerRoute();
            });
        }


        private void RegisterEventPusPublisher(IServiceCollection services)
        {
            services.AddSingleton<IEventBrokerPublisher, RabbitMQEventBrokerPublisher>(sp =>
                {
                    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                  
                    var logger = sp.GetRequiredService<ILogger<RabbitMQEventBrokerPublisher>>();
                   
                    var retryCount = 5;
                    if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                    {
                        retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                    }

                    return new RabbitMQEventBrokerPublisher(rabbitMQPersistentConnection, logger, retryCount);
                });
        }

        /// <summary>
        /// this will be overridden during Integration tests
        /// </summary>
        /// <param name="services"></param>
        protected virtual void ConfigureAuth(IServiceCollection services)
        {

            IdentityModelEventSource.ShowPII = true;

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.BackchannelHttpHandler = new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
                    };
                    //var key = new JsonWebKey(File.ReadAllText(@"tempkey.jwk"));
                    //options.TokenValidationParameters=new TokenValidationParameters()
                    //{
                    //    IssuerSigningKey = key
                    //};
                    options.Authority = Configuration["Authority"];
                    options.Audience = "PaymentGateway";
                });
        }

    }
}
