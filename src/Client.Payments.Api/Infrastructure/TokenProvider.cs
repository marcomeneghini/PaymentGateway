using Client.Payments.Api.Domain;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

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
          
            //uses the http client configured to bypass ssl
            var client = _httpClientFactory.CreateClient("HttpClientWithSSLUntrusted");
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
    }
}
