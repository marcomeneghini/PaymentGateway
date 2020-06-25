using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Infrastructure;
using PaymentGateway.Api.Middlewares;
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

            
            services.AddSingleton<IMerchantRepository, InMemoryMerchantRepository>();
            services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICipherService, AesCipherService>();

           

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

       
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerfactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerfactory.AddSerilog();

            app.UseMiddleware(typeof(ExceptionMiddleware));
            app.UseMiddleware(typeof(RequestIdLoggingMiddleware));

            app.UseRouting();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentGateway Demo V1");
            });

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

        //private void RegisterEventBus(IServiceCollection services)
        //{
        //    var subscriptionClientName = Configuration["SubscriptionClientName"];


        //    services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        //    {
        //        var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
        //        var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
        //        var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
        //        var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

        //        var retryCount = 5;
        //        if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
        //        {
        //            retryCount = int.Parse(Configuration["EventBusRetryCount"]);
        //        }

        //        return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
        //    });


        //    services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        //    services.AddTransient<ProductPriceChangedIntegrationEventHandler>();
        //    services.AddTransient<OrderStartedIntegrationEventHandler>();
        //}


    }
}
