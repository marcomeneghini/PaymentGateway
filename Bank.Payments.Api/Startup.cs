using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bank.Payments.Api.Domain;
using Bank.Payments.Api.Infrastructure;
using Bank.Payments.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bank.Payments.Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // this lives for the entire duration of the service
            services.AddSingleton<IPaymentRepository,InMemoryPaymentRepository>();
            // these live for the entire duration of an Http call
            services.AddScoped<ICardPaymentService, CardPaymentService>();
            services.AddScoped<IBankAccountRepository, FakeBankAccountRepository>();
            services.AddScoped<ICardRepository, FakeCardRepository>();

            services.AddAutoMapper(typeof(Startup));
            services.AddControllers();
            services.AddHealthChecks();
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
