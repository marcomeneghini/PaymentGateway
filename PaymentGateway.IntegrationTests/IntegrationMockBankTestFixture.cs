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

namespace PaymentGateway.IntegrationTests
{
    public class IntegrationMockBankTestFixture<TStartupPgApi, TStartupPgProcApi> : IDisposable
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

        public HttpClient PgApiClient { get; private set; }
        public HttpClient PgProcApiClient { get; private set; }

        #region Constructors

        public IntegrationMockBankTestFixture() : this(Path.Combine(""))
        {

        }

        protected IntegrationMockBankTestFixture(string relativeTargetProjectParentDir)
        {
            InitializePaymentGatewayApiTestServer(relativeTargetProjectParentDir);
            InitializePaymentProcessorApiTestServer(relativeTargetProjectParentDir);
        }

        #endregion


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

        private void InitializePaymentProcessorApiTestServer(string relativeTargetProjectParentDir)
        {
            var fakeSucceededCardPaymentResponse = Helper.CreateFake_Succeeded_CardPaymentResponse();
            var mockBankPaymentProxy = Mock.Of<IBankPaymentProxy>();
            Mock.Get(mockBankPaymentProxy)
                .Setup(m => m.CreatePaymentAsync(It.IsAny<CardPaymentRequest>())).ReturnsAsync(fakeSucceededCardPaymentResponse);

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
                    services.TryAddTransient(sp => mockBankPaymentProxy);
                })
                .UseConfiguration(configurationBuilder.Build())
                .UseEnvironment("Development")
                .UseStartup(typeof(TStartupPgProcApi));

            // Create instance of test server
            PgProcApiServer = new TestServer(webHostBuilder);

            // Add configuration for client
            PgProcApiClient = PgApiServer.CreateClient();
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

        public void Dispose()
        {
        }
    }
}
