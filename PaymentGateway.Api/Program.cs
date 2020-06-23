using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Linq;

namespace PaymentGateway.Api
{
    public class Program
    {
        private static string _environmentName;
        public static void Main(string[] args)
        {
            CreateWebHost(args).Run();
           //CreateHostBuilder(args).Build().Run();
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });
        public static IWebHost CreateWebHost(string[] args)
        {
            var webHost = BuildWebHost(args);


            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()  // tell serilog to get values from LogContext
                .ReadFrom.Configuration(configuration)
                .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("swagger")))
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
