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
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile() // <-- usefull
            };
        public static IEnumerable<ApiResource> GetApiResources() =>
            new List<ApiResource> {
                new ApiResource()
                {
                    Name = "PaymentGateway",
                    Scopes = new List<string>(){"CreatePaymentScope"},
                    ShowInDiscoveryDocument = true
                   
                }
                
            };
        public static IEnumerable<ApiScope> GetApiScopes() =>
            new List<ApiScope> {
                new ApiScope("CreatePaymentScope")
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client> {
                new Client {
                    ClientId = "amazonId",
                    ClientSecrets = { new Secret("amazonSecret".ToSha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    
                    AllowedScopes = { "CreatePaymentScope" }
                },

            };
    }
}
