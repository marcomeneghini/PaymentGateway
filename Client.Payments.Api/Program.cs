using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Client.Payments.Api
{
    public class Program
    {
        private static string _environmentName;

        public static void Main(string[] args)
        {
            CreateWebHost(args).Run();
        }

        public static IWebHost CreateWebHost(string[] args)
        {
            var webHost = BuildWebHost(args);


            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()  // tells serilog to get values from LogContext for common values
                .ReadFrom.Configuration(configuration)
                //.Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("swagger")))
                .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("healthcheck")))
                .CreateLogger();


            return webHost;
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    if (env.IsDevelopment())
                    {
                        config.AddUserSecrets<Startup>();
                    }
                })
                .ConfigureLogging((hostingContext, config) =>
                {
                    config.ClearProviders();
                    _environmentName = hostingContext.HostingEnvironment.EnvironmentName;

                })
                .UseStartup<Startup>()
                .UseSerilog() // VERY IMPORTANT!!
                .Build();
        }
    }
}
