using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.Processor.Api.Proxies;

namespace PaymentGateway.IntegrationTests
{
    public class IntegrationTestServerBankTestFixture<TStartupPgApi, TStartupPgProcApi, TStartupBankApi> : IDisposable
    {

        public static string GetProjectPath(string projectRelativePath, Assembly startupAssembly)
        {
            var projectName = startupAssembly.GetName().Name;

            var applicationBasePath = AppContext.BaseDirectory;

            var directoryInfo = new DirectoryInfo(applicationBasePath);

            do
            {
                directoryInfo = directoryInfo.Parent;

                var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));

                if (projectDirectoryInfo.Exists)
                    if (new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName, $"{projectName}.csproj"))
                        .Exists)
                        return Path.Combine(projectDirectoryInfo.FullName, projectName);
            } while (directoryInfo.Parent != null);

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
        }

        private TestServer PgApiServer;
        private TestServer PgProcApiServer;
        private TestServer BankApiServer;
        public HttpClient PgApiClient { get; private set; }
        public HttpClient PgProcApiClient { get; private set; }

        public HttpClient BankApiClient { get; private set; }

        #region Constructors

        public IntegrationTestServerBankTestFixture() : this(Path.Combine(""))
        {

        }

        protected IntegrationTestServerBankTestFixture(string relativeTargetProjectParentDir)
        {
            InitializePaymentGatewayApiTestServer(relativeTargetProjectParentDir);

            var bankClient = InitializeBankApiTestServer(relativeTargetProjectParentDir);

            InitializePaymentProcessorApiTestServer(relativeTargetProjectParentDir, bankClient);
        }

        #endregion

        private HttpClient InitializeBankApiTestServer(string relativeTargetProjectParentDir)
        {
            var startupAssembly = typeof(TStartupBankApi).GetTypeInfo().Assembly;
            var contentRoot = GetProjectPath(relativeTargetProjectParentDir, startupAssembly);

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile("appsettings.json");

            var webHostBuilder = new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .ConfigureServices(InitializeServicesBankSimulatorApi)
                .UseConfiguration(configurationBuilder.Build())
                .UseEnvironment("Development")
                .UseStartup(typeof(TStartupBankApi));

            // Create instance of test server
            BankApiServer = new TestServer(webHostBuilder);

            // Add configuration for client
            BankApiClient = PgApiServer.CreateClient();
            BankApiClient.BaseAddress = new Uri("http://localhost:6000");
            BankApiClient.DefaultRequestHeaders.Accept.Clear();
            BankApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return BankApiClient;
        }

        private void InitializePaymentGatewayApiTestServer(string relativeTargetProjectParentDir)
        {
            var startupAssembly = typeof(TStartupPgApi).GetTypeInfo().Assembly;
            var contentRoot = GetProjectPath(relativeTargetProjectParentDir, startupAssembly);

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile("appsettings.json");

            var webHostBuilder = new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .ConfigureServices(InitializeServicesPaymentGatewayApi)
                .UseConfiguration(configurationBuilder.Build())
                .UseEnvironment("Development")
                .UseStartup(typeof(TStartupPgApi));

            // Create instance of test server
            PgApiServer = new TestServer(webHostBuilder);

            // Add configuration for client
            PgApiClient = PgApiServer.CreateClient();
            PgApiClient.BaseAddress = new Uri("http://localhost:6001");
            PgApiClient.DefaultRequestHeaders.Accept.Clear();
            PgApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void InitializePaymentProcessorApiTestServer(string relativeTargetProjectParentDir, HttpClient bankHttpClient)
        {
           
          
           
            var startupAssembly = typeof(TStartupPgProcApi).GetTypeInfo().Assembly;
            var contentRoot = GetProjectPath(relativeTargetProjectParentDir, startupAssembly);

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile("appsettings.json");

            var webHostBuilder = new WebHostBuilder()
                .UseContentRoot(contentRoot)
                 .ConfigureServices(InitializeServicesPaymentProcessorApi)
                .ConfigureTestServices(services => {
                    services.RemoveAll<IBankPaymentProxy>();
                    services.AddHttpClient<IBankPaymentProxy, MyBankPaymentProxy>(client =>
                        {
                            client = bankHttpClient;
                        }
                    );
                })
                .UseConfiguration(configurationBuilder.Build())
                .UseEnvironment("Development")
                .UseStartup(typeof(TStartupPgProcApi));

            // Create instance of test server
            PgProcApiServer = new TestServer(webHostBuilder);

            // Add configuration for client
            PgProcApiClient = PgProcApiServer.CreateClient();
            PgProcApiClient.BaseAddress = new Uri("http://localhost:6002");
            PgProcApiClient.DefaultRequestHeaders.Accept.Clear();
            PgProcApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }



        protected virtual void InitializeServicesPaymentGatewayApi(IServiceCollection services)
        {

            var startupAssembly = typeof(TStartupPgApi).GetTypeInfo().Assembly;

            var manager = new ApplicationPartManager
            {
                ApplicationParts =
                {
                    new AssemblyPart(startupAssembly)
                },
                FeatureProviders =
                {
                    new ControllerFeatureProvider(),
                }
            };

            services.AddSingleton(manager);
        }

        protected virtual void InitializeServicesPaymentProcessorApi(IServiceCollection services)
        {

            var startupAssembly = typeof(TStartupPgProcApi).GetTypeInfo().Assembly;

            var manager = new ApplicationPartManager
            {
                ApplicationParts =
                {
                    new AssemblyPart(startupAssembly)
                },
                FeatureProviders =
                {
                    new ControllerFeatureProvider(),
                }
            };

            services.AddSingleton(manager);
        }

        protected virtual void InitializeServicesBankSimulatorApi(IServiceCollection services)
        {

            var startupAssembly = typeof(TStartupBankApi).GetTypeInfo().Assembly;

            var manager = new ApplicationPartManager
            {
                ApplicationParts =
                {
                    new AssemblyPart(startupAssembly)
                },
                FeatureProviders =
                {
                    new ControllerFeatureProvider(),
                }
            };

            services.AddSingleton(manager);
        }

        public void Dispose()
        {
        }
    }
}
