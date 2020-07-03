using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PaymentGateway.Api
{
   public  class TestStartupNoAuth : Startup
    {
        public TestStartupNoAuth(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureAuth(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test Scheme"; // has to match scheme in TestAuthenticationExtensions
                options.DefaultChallengeScheme = "Test Scheme";
            }).AddTestAuth(o => { });
        }
    }
}
