using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaymentGateway.Api.Attributes;
using PaymentGateway.Api.Infrastructure;
using PaymentGateway.Api.Models;
using Xunit;

namespace PaymentGateway.Api.IntegrationTests
{
    public class MerchantCardPaymentsTests: IClassFixture<TestFixture<Startup>>
    {
        private HttpClient Client;

        //private Guid unknownMerchantGuid = new Guid("00092C77-3C0E-447C-ABC5-0AF6CF829A22");
        //private Guid amazonValidMerchantGuid = new Guid("53D92C77-3C0E-447C-ABC5-0AF6CF829A22");
        //private Guid appleInValidMerchantGuid = new Guid("11112C77-3C0E-447C-ABC5-0AF6CF821111");
        private Guid unknownMerchantGuid = new Guid("00092C77-3C0E-447C-ABC5-0AF6CF829A22");
        private Guid amazonValidMerchantGuid = InMemoryMerchantRepository.CreateMerchant_Amazon().Id;
        private Guid appleInValidMerchantGuid = InMemoryMerchantRepository.CreateMerchant_InvalidApple().Id;
        private string invalidGuid = "sssss00092C77-3C0E-447C-ABC5-0AF6CF829A22";
        public MerchantCardPaymentsTests(TestFixture<Startup> fixture)
        {
            Client = fixture.Client;
        }

        [Fact]
        public async Task TestCreatePayment_John_ValidAmazon_Async()
        {
            // Arrange
            var request = new
            {
                Url = "/api/merchantcardpayments",
                Body = new
                {
                    MerchantId = amazonValidMerchantGuid.ToString(),
                    RequestId = Guid.NewGuid().ToString(),
                    CardNumber = "1298 1234 1234 1234",
                    CardHolderName = "John Doe",
                    MonthExpiryDate = 1,
                    YearExpiryDate = 2021,
                    CVV = "555",
                    Currency = "GBP",
                    Amount = 10
                }
            };

            // Act
            var response = await Client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestCreatePayment_John_InValidApple_Async()
        {
            // Arrange
            var request = new
            {
                Url = "/api/merchantcardpayments",
                Body = new
                {
                    MerchantId = appleInValidMerchantGuid.ToString(),
                    RequestId = Guid.NewGuid().ToString(),
                    CardNumber = "1234 1234 1234 1234",
                    CardHolderName = "John Doe",
                    MonthExpiryDate = 1,
                    YearExpiryDate = 2021,
                    CVV = "555",
                    Currency="GBP",
                    Amount = 10,
                }
            };

            // Act
            var response = await Client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestCreatePayment_John_Unknown_NotFound404_Async()
        {
            // Arrange
            var request = new
            {
                Url = "/api/merchantcardpayments",
                Body = new
                {
                    MerchantId = unknownMerchantGuid.ToString(),
                    RequestId = Guid.NewGuid().ToString(),
                    CardNumber = "1298 1234 1234 1234",
                    CardHolderName = "John Doe",
                    MonthExpiryDate = 1,
                    YearExpiryDate = 2021,
                    CVV = "555",
                    Currency = "GBP",
                    Amount = 10
                }
            };

            // Act
            var response = await Client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var stringvalue = await response.Content.ReadAsStringAsync();
            // try to deserialize
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponseModel>(stringvalue);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            
        }

        [Fact]
        public async Task TestCreatePayment_John_ValidAmazon_Conflict409_Async()
        {
            // Arrange
            var request = new
            {
                Url = "/api/merchantcardpayments",
                Body = new
                {
                    MerchantId = amazonValidMerchantGuid.ToString(),
                    RequestId = "request1",
                    CardNumber = "1298 1234 1234 1234",
                    CardHolderName = "John Doe",
                    MonthExpiryDate = 1,
                    YearExpiryDate = 2021,
                    CVV = "555",
                    Currency = "GBP",
                    Amount = 10
                }
            };
            var request2 = new
            {
                Url = "/api/merchantcardpayments",
                Body = new
                {
                    MerchantId = amazonValidMerchantGuid.ToString(),
                    RequestId = "request1",
                    CardNumber = "1234 1234 1234 1234",
                    CardHolderName = "John Doe",
                    MonthExpiryDate = 1,
                    YearExpiryDate = 2021,
                    CVV = "555",
                    Amount = 10
                }
            };

            // Act
            var response = await Client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var stringvalue = await response.Content.ReadAsStringAsync();

            // Act
            var response2 = await Client.PostAsync(request2.Url, ContentHelper.GetStringContent(request.Body));
            var stringvalue2 = await response.Content.ReadAsStringAsync();
            // try to deserialize
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponseModel>(stringvalue2);
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);
        }

        [Fact]
        public async Task TestCreatePayment_John_ValidAmazon_UnprocessableEntity422_Async()
        {
            // Arrange
            var request = new
            {
                Url = "/api/merchantcardpayments",
                Body = new
                {
                    MerchantId = invalidGuid,
                    RequestId = "request1",
                    CardNumber = "1298 1234 1234 1234",
                    CardHolderName = "John Doe",
                    MonthExpiryDate = 1,
                    YearExpiryDate = 2021,
                    CVV = "555",
                    Currency = "GBP",
                    Amount = 10
                }
            };
         

            // Act
            var response = await Client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var stringvalue = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            // try to deserialize
            var errorResponse = JsonConvert.DeserializeObject<ValidationResultModel>(stringvalue);

        }
    }
}
