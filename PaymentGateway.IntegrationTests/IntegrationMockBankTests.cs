using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PaymentGateway.Api.Models;
using PaymentGateway.Processor.Api.Models;
using Xunit;
using PgApi =PaymentGateway.Api ;
using PgProcessorApi = PaymentGateway.Processor.Api;
namespace PaymentGateway.IntegrationTests
{
    public  class IntegrationMockBankTests : IClassFixture<IntegrationMockBankTestFixture<PgApi.Startup, PgProcessorApi.Startup>>
    {
        private HttpClient PgApiClient;
        private HttpClient PgProcApiClient;


        private Guid unknownMerchantGuid = new Guid("00092C77-3C0E-447C-ABC5-0AF6CF829A22");
        private Guid amazonValidMerchantGuid = new Guid("53D92C77-3C0E-447C-ABC5-0AF6CF829A22");
        private Guid appleInValidMerchantGuid = new Guid("11112C77-3C0E-447C-ABC5-0AF6CF821111");


        public IntegrationMockBankTests(IntegrationMockBankTestFixture<PgApi.Startup, PgProcessorApi.Startup> fixture)
        {
            PgApiClient = fixture.PgApiClient;
            PgProcApiClient = fixture.PgProcApiClient;
        }

        [Fact]
        public async Task TestE2E_CreatePayment_John_ValidAmazon_Async()
        {
            // Arrange
            var request = new
            {
                Url = "/api/merchantcardpayments",
                Body = new
                {
                    MerchantId = amazonValidMerchantGuid.ToString(),
                    RequestId = Guid.NewGuid().ToString(),
                    CardNumber = "1234 1234 1234 1234",
                    CardHolderName = "John Doe",
                    MonthExpiryDate = 1,
                    YearExpiryDate = 2021,
                    CVV = "555",
                    Amount = 10
                }
            };

            // Act
            var merchantPaymentHttpResponse = await PgApiClient.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            merchantPaymentHttpResponse.EnsureSuccessStatusCode();
            var merchantPaymentResponseString = await merchantPaymentHttpResponse.Content.ReadAsStringAsync();
            var merchantPaymentResponse= JsonConvert.DeserializeObject<CreatePaymentResponseModel>(merchantPaymentResponseString);

            PaymentStatusModel paymentStatus=new PaymentStatusModel(){Status = ""};
            // loop to get the value of the payment status from the PaymentGateway.Processor
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    // Arrange
                    var paymetProcessorRequest = $"/api/PaymentStatuses?paymentId={merchantPaymentResponse.PaymentRequestId}";

                    // Act
                    var response = await PgProcApiClient.GetAsync(paymetProcessorRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        var stringResponse = await response.Content.ReadAsStringAsync();
                        paymentStatus = JsonConvert.DeserializeObject<PaymentStatusModel>(stringResponse);
                        break;
                    }
                    await Task.Delay(2000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            Assert.Equal(paymentStatus.Status, PaymentStatusEnum.Completed.ToString());
        }

        [Fact]
        public async Task TestE2E_CreatePayment_WrongCard_ValidAmazon_Async()
        {
            // Arrange
            var request = new
            {
                Url = "/api/merchantcardpayments",
                Body = new
                {
                    MerchantId = amazonValidMerchantGuid.ToString(),
                    RequestId = Guid.NewGuid().ToString(),
                    CardNumber = "---- 1234 1234 1234",
                    CardHolderName = "John Doe",
                    MonthExpiryDate = 1,
                    YearExpiryDate = 2021,
                    CVV = "555",
                    Amount = 10
                }
            };

            // Act
            var merchantPaymentHttpResponse = await PgApiClient.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            merchantPaymentHttpResponse.EnsureSuccessStatusCode();
            var merchantPaymentResponseString = await merchantPaymentHttpResponse.Content.ReadAsStringAsync();
            var merchantPaymentResponse = JsonConvert.DeserializeObject<CreatePaymentResponseModel>(merchantPaymentResponseString);

            PaymentStatusModel paymentStatus = new PaymentStatusModel() { Status = "" };
            // loop to get the value of the payment status from the PaymentGateway.Processor
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    // Arrange
                    var paymetProcessorRequest = $"/api/PaymentStatuses?paymentId={merchantPaymentResponse.PaymentRequestId}";

                    // Act
                    var response = await PgProcApiClient.GetAsync(paymetProcessorRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        var stringResponse = await response.Content.ReadAsStringAsync();
                        paymentStatus = JsonConvert.DeserializeObject<PaymentStatusModel>(stringResponse);
                        break;
                    }
                    await Task.Delay(2000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            Assert.Equal(paymentStatus.Status, PaymentStatusEnum.Error.ToString());
        }
    }
}
