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
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.Processor.Api.Domain.Entities;
using PaymentGateway.Processor.Api.Infrastructure;

namespace PaymentGateway.Processor.Api.IntegrationTests
{
    public class TestFixture<TStartupNoAuth> : IDisposable
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
                    if (new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName, $"{projectName}.csproj")).Exists)
                        return Path.Combine(projectDirectoryInfo.FullName, projectName);
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
        }

        private TestServer Server;

        public TestFixture()
            : this(Path.Combine(""))
        {
        }

        public HttpClient Client { get; }

        public void Dispose()
        {
            Client.Dispose();
            Server.Dispose();
        }

        protected virtual void InitializeServices(IServiceCollection services)
        {

            var startupAssembly = typeof(TStartupNoAuth).GetTypeInfo().Assembly;

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

        protected TestFixture(string relativeTargetProjectParentDir)
        {
            var fakeSucceededCardPaymentResponse = Helper.CreateFake_Succeeded_CardPaymentResponse("request1");
            var mockBankPaymentProxy = Mock.Of<IBankPaymentProxy>();
            Mock.Get(mockBankPaymentProxy)
                .Setup(m => m.CreatePaymentAsync(It.IsAny<CardPayment>())).ReturnsAsync(fakeSucceededCardPaymentResponse);


            var startupAssembly = typeof(TStartupNoAuth).GetTypeInfo().Assembly;
            var contentRoot = GetProjectPath(relativeTargetProjectParentDir, startupAssembly);

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile("appsettings.json");

            var webHostBuilder = new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .ConfigureTestServices(services => {
                    services.RemoveAll<IBankPaymentProxy>();
                    services.TryAddTransient(sp => mockBankPaymentProxy);
                    services.RemoveAll<IPaymentStatusRepository>();
                    services.TryAddScoped<IPaymentStatusRepository,InMemoryPaymentStatusRepository>();
                })
                .UseConfiguration(configurationBuilder.Build())
                .UseEnvironment("Development")
                .UseStartup(typeof(TStartupNoAuth));

            // Create instance of test server
            Server = new TestServer(webHostBuilder);

            // Add configuration for client
            Client = Server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost:10000");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

    }
}
