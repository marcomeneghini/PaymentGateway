using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Channels;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.Processor.Api.Infrastructure;
using PaymentGateway.Processor.Api.Messaging;
using PaymentGateway.Processor.Api.Proxies;
using PaymentGateway.SharedLib.Encryption;
using PaymentGateway.SharedLib.EventBroker;
using PaymentGateway.SharedLib.Messages;
using RabbitMQ.Client;

namespace PaymentGateway.Processor.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<IPaymentStatusRepository, InMemoryPaymentStatusRepository>();
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
            services.AddHttpClient<IBankPaymentProxy, MyBankPaymentProxy>(client =>
                {
                   //var bankConfiguration = new BankPaymentConfiguration();
                    //Configuration.GetSection(BankPaymentConfiguration.SectionName).Bind(bankConfiguration);

                    client.BaseAddress = new Uri( Configuration["BankPaymentsAddress"]);
                    //client.BaseAddress = new Uri(bankConfiguration.BaseAddress);

                    client.DefaultRequestHeaders.Accept.Clear();

                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                }
            );
            var channel = Channel.CreateBounded<EncryptedMessage>(100);
            services.AddSingleton(channel);

            services.AddSingleton<IEventBrokerSubscriber, RabbitMQEventBrokerSubscriber>();
            services.AddSingleton<ICipherService, AesCipherService>();
            services.AddSingleton<IChannelProducer>(ctx => {
                //channel = ctx.GetRequiredService<Channel<EncryptedMessage>>();
                var logger = ctx.GetRequiredService<ILogger<ChannelProducer>>();
                var paymentRepository = ctx.GetRequiredService<IPaymentStatusRepository>();
                var cipherService = ctx.GetRequiredService<ICipherService>();
                return new ChannelProducer(channel.Writer, paymentRepository, cipherService ,logger);
            });

            services.AddSingleton<IChannelConsumer>(ctx => {
                //var innerChannelChannel = ctx.GetRequiredService<Channel<EncryptedMessage>>();
                var logger = ctx.GetRequiredService<ILogger<ChannelConsumer>>();
                var paymentRepository = ctx.GetRequiredService<IPaymentStatusRepository>();
                var bankPaymentProxy = ctx.GetRequiredService<IBankPaymentProxy>();
                var cipherService = ctx.GetRequiredService<ICipherService>();
                var mapperService = ctx.GetRequiredService<IMapper>();

                return new ChannelConsumer(channel.Reader, logger, paymentRepository, bankPaymentProxy, cipherService, mapperService);
            });

            services.AddHostedService<EventBrokerBackgroundWorker>();

            services.AddControllers();
           
            services.AddHealthChecks();
            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
