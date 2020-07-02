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

        public  async Task<string> CustomGetApiToken()
        {
        
            var identityServerAddress = _configuration["IdentityServer"];
            var clientId = _configuration["ClientId"];
            var clientSecret = _configuration["ClientSecret"];
            using (var client = new HttpClient())
            {
                //setup client
                client.BaseAddress = new Uri(identityServerAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //setup login data
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("scope", "CreatePaymentScope")
                });

                //send request
                try
                {
                    var responseMessage = await client.PostAsync("connect/token", formContent);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        //get access token from response body
                        var responseJson = responseMessage.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(responseJson);
                        return jObject.GetValue("access_token").ToString();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
             
                return string.Empty;
            }
        }

        public async Task<string> GetAccessToken__()
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

        private  SocketsHttpHandler GetHandler(X509Certificate2 certificate)
        {
            var handler = new SocketsHttpHandler();
            handler.SslOptions.ClientCertificates = new X509CertificateCollection { certificate };

            return handler;
        }

        private  X509Certificate2 CreateClientCertificate(string name)
        {
            X500DistinguishedName distinguishedName = new X500DistinguishedName($"CN={name}");

            using (RSA rsa = RSA.Create(2048))
            {
                var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                request.CertificateExtensions.Add(
                    new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));

                request.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(
                        new OidCollection { new Oid("1.3.6.1.5.5.7.3.2") }, false));

                return request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.AddDays(-1)), new DateTimeOffset(DateTime.UtcNow.AddDays(10)));
            }
        }
    }
}
