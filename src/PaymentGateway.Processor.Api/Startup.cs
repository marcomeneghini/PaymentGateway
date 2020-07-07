using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Channels;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.Processor.Api.Infrastructure;
using PaymentGateway.Processor.Api.Messaging;
using PaymentGateway.Processor.Api.Middleware;
using PaymentGateway.Processor.Api.Proxies;
using PaymentGateway.SharedLib.Encryption;
using PaymentGateway.SharedLib.EventBroker;
using PaymentGateway.SharedLib.Messages;
using Prometheus;
using RabbitMQ.Client;
using Serilog;

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
            ConfigureAuth(services);
           
            // Add entity entity framework .
            var sqlConnectionString = Configuration.GetConnectionString("SqlConnection");
            services.AddDbContext<PaymentGatewayProcessorDbContext>(options => options.UseSqlServer(sqlConnectionString).EnableSensitiveDataLogging());
            services.AddHttpClient();
            services.AddScoped<IPaymentStatusRepository, EfPaymentStatusRepository>();
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = string.IsNullOrEmpty(Configuration["EventBrokerConnection"])?"rabbitmq": Configuration["EventBrokerConnection"],
                    DispatchConsumersAsync = true
                };
                // set a default 
                if (!string.IsNullOrEmpty(Configuration["EventBrokerUserName"]))
                {
                    factory.UserName = Configuration["EventBrokerUserName"];
                }
                // set a default 
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
            //services.AddTransient<IBankPaymentProxy,FakeBankPaymentProxy>();

            services.AddHttpClient<IBankPaymentProxy, MyBankPaymentProxy>(client =>
                {
                    var bankPaymentsAddress = Configuration["BankPaymentsAddress"];
                    client.BaseAddress = new Uri(bankPaymentsAddress);

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
                var logger = ctx.GetRequiredService<ILogger<ChannelProducer>>();
                var cipherService = ctx.GetRequiredService<ICipherService>();
                return new ChannelProducer(channel.Writer, cipherService ,logger);
            });

            services.AddSingleton<IChannelConsumer>(ctx => {
                //var innerChannelChannel = ctx.GetRequiredService<Channel<EncryptedMessage>>();
                var logger = ctx.GetRequiredService<ILogger<ChannelConsumer>>();
                var bankPaymentProxy = ctx.GetRequiredService<IBankPaymentProxy>();
                var cipherService = ctx.GetRequiredService<ICipherService>();
                var mapperService = ctx.GetRequiredService<IMapper>();

                return new ChannelConsumer(channel.Reader, logger, bankPaymentProxy, cipherService, mapperService);
            });

            services.AddHostedService<EventBrokerBackgroundWorker>();

            services.AddControllers();
           
            services.AddHealthChecks();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PaymentGateway Processor Demo", Version = "v1" });
            });
            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerfactory)
        {

            // Custom Metrics to count requests for each endpoint and the method
            var counter = Metrics.CreateCounter("paymentgateway_processor_counter", "Counts requests to the processor endpoints", new CounterConfiguration
            {
                LabelNames = new[] { "method", "endpoint" }
            });
            app.Use((context, next) =>
            {
                counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
                return next();
            });
            app.UseMiddleware(typeof(ExceptionMiddleware));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            loggerfactory.AddSerilog();


            // Use the Prometheus middleware
            app.UseMetricServer();
            app.UseHttpMetrics();
            app.UseRouting();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentGateway Processor Demo V1");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapDefaultControllerRoute();
            });
        }

        protected virtual void ConfigureAuth(IServiceCollection services)
        {

            IdentityModelEventSource.ShowPII = true;

            services.AddAuthentication(cfg =>
                {
                    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer( options =>
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
                    options.Audience = "Processor";
                });
        }
    }
}
