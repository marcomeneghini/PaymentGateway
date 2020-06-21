using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Infrastructure;
using PaymentGateway.Api.Services;
using PaymentGateway.SharedLib.Encryption;

namespace PaymentGateway.Api
{
    public class Startup
    {
      
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IMerchantRepository, EfInMemoryMerchantRepository>();
            services.AddScoped<IPaymentRepository, EfInMemoryPaymentRepository>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICipherService, AesCipherService>();

            services.AddControllers();
            services.AddDbContext<PaymentGatewayDbContext>(opt => opt.UseInMemoryDatabase("PaymentGatewayApiDatabase"));
            services.AddHealthChecks();
        }

       
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var context = app.ApplicationServices.GetService<PaymentGatewayDbContext>();
            AddTestData(context);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapDefaultControllerRoute();
            });
        }

        private static void AddTestData(PaymentGatewayDbContext context)
        {
           

            context.Merchants.Add(EfInMemoryMerchantRepository.CreateMerchant_Amazon());
            context.Merchants.Add(EfInMemoryMerchantRepository.CreateMerchant_Apple());

          

            context.SaveChanges();
        }
    }
}
