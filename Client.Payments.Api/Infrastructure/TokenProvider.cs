using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Client.Payments.Api.Domain;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Client.Payments.Api.Infrastructure
{
    public class TokenProvider:ITokenProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public TokenProvider(IHttpClientFactory httpClientFactory,IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string> GetAccessToken()
        {
            var identityServerAddress = _configuration["IdentityServer"];
            
            using (var httpClientHandler = new HttpClientHandler())
            {

                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    return true;
                };
                using (var client = new HttpClient(httpClientHandler))
                {
                    try
                    {
                        var discoveryDocument = await client.GetDiscoveryDocumentAsync(identityServerAddress);
                        var clientId = _configuration["ClientId"];
                        var clientSecret = _configuration["ClientSecret"];
                        var tokenResponse = await client.RequestClientCredentialsTokenAsync(
                            new ClientCredentialsTokenRequest
                            {
                                Address = discoveryDocument.TokenEndpoint,
                                ClientId = clientId,
                                ClientSecret = clientSecret,
                                
                                Scope = "CreatePaymentScope"
                            });
                        return tokenResponse.AccessToken;
                    }
                    catch (HttpRequestException ex)
                    {
                        var c = ex;
                        throw;
                    }
                }
            }

        }

      

        public async Task<string> GetAccessToken_afterConfiguringSSL()
        {
            var identityServerAddress = _configuration["IdentityServer"];
           
            try
            {
                var client = _httpClientFactory.CreateClient();
                var discoveryDocument = await client.GetDiscoveryDocumentAsync(identityServerAddress);
                var clientId = _configuration["ClientId"];
                var clientSecret = _configuration["ClientSecret"];
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(
                    new ClientCredentialsTokenRequest
                    {
                        Address = discoveryDocument.TokenEndpoint,

                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        Scope = "PaymentGateway.Api"
                    });
                return tokenResponse.AccessToken;
            }
            catch (HttpRequestException ex)
            {
                var c = ex;
                throw;
            }
        }

      
    }
}
