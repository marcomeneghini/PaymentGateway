using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;

namespace Company.IdentityServer
{
    public static class StaticInitializer
    {
        public static IEnumerable<ApiResource> GetApiResources() =>
            new List<ApiResource> {
                new ApiResource("PaymentGateway.Api"),
                new ApiResource("PaymentGateway.Processor.Api")
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client> {
                new Client {
                    ClientId = "amazon_id",
                    ClientSecrets = { new Secret("amazonSecret".ToSha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    AllowedScopes = { "PaymentGateway.Api", "PaymentGateway.Processor.Api" }
                },

            };
    }
}
