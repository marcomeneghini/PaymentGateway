using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Company.IdentityServer
{
    public class Startup
    {
     
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
           // services.AddHealthChecks();

            services.AddIdentityServer()
                .AddInMemoryApiResources(StaticInitializer.GetApiResources()) // list of resources/services available 
                //.AddInMemoryIdentityResources(StaticInitializer.GetIdentityResources())
                .AddInMemoryClients(StaticInitializer.GetClients()) // list of the allowed clients
                .AddInMemoryApiScopes(StaticInitializer.GetApiScopes())
                .AddDeveloperSigningCredential();      
            // dev certificate

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
               // endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}
