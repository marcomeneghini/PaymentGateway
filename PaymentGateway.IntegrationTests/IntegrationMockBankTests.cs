using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PaymentGateway.Api.Infrastructure;
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
        private Guid amazonValidMerchantGuid = InMemoryMerchantRepository.CreateMerchant_Amazon().Id;
        private Guid appleInValidMerchantGuid = InMemoryMerchantRepository.CreateMerchant_InvalidApple().Id;


        public IntegrationMockBankTests(IntegrationMockBankTestFixture<PgApi.Startup, PgProcessorApi.Startup> fixture)
        {
            PgApiClient = fixture.PgApiClient;
            PgProcApiClient = fixture.PgProcApiClient;
        }

        [Fact]
        public async Task TestE2E_CreatePayment_John_ValidAmazon_Async()
        {
            var johnDoeCard = Helper.GenerateCard_JohnDoe();
            // Arrange
            var request = new
            {
                Url = "/api/merchantcardpayments",
                Body = new
                {
                    MerchantId = amazonValidMerchantGuid.ToString(),
                    RequestId = Guid.NewGuid().ToString(),
                    CardNumber = johnDoeCard.CardNumber,
                    CardHolderName = johnDoeCard.CardHolderName,
                    MonthExpiryDate = johnDoeCard.MonthExpiryDate,
                    YearExpiryDate = johnDoeCard.YearExpiryDate,
                    CVV = johnDoeCard.CVV,
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
            Assert.Equal(PaymentStatusEnum.Completed.ToString(),paymentStatus.Status);
        }

        [Fact]
        public async Task TestE2E_CreatePayment_Jane_ValidAmazon_Async()
        {
            var janeDoeCard = Helper.GenerateCard_JaneDoe();
            // Arrange
            var request = new
            {
                Url = "/api/merchantcardpayments",
                Body = new
                {
                    MerchantId = amazonValidMerchantGuid.ToString(),
                    RequestId = Guid.NewGuid().ToString(),
                    CardNumber = janeDoeCard.CardNumber,
                    CardHolderName = janeDoeCard.CardHolderName,
                    MonthExpiryDate = janeDoeCard.MonthExpiryDate,
                    YearExpiryDate = janeDoeCard.YearExpiryDate,
                    CVV = janeDoeCard.CVV,
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
            Assert.Equal(PaymentStatusEnum.Completed.ToString(), paymentStatus.Status);
        }

        [Fact]
        public async Task TestE2E_CreatePayment_John_InValidApple_Async()
        {
            var johnDoeCard = Helper.GenerateCard_JohnDoe();
            // Arrange
            var request = new
            {
                Url = "/api/merchantcardpayments",
                Body = new
                {
                    MerchantId = appleInValidMerchantGuid.ToString(),
                    RequestId = Guid.NewGuid().ToString(),
                    CardNumber = johnDoeCard.CardNumber,
                    CardHolderName = johnDoeCard.CardHolderName,
                    MonthExpiryDate = johnDoeCard.MonthExpiryDate,
                    YearExpiryDate = johnDoeCard.YearExpiryDate,
                    CVV = johnDoeCard.CVV,
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
            Assert.Equal(PaymentStatusEnum.Error.ToString(), paymentStatus.Status);
            Assert.Equal(request.Body.RequestId, paymentStatus.RequestId);
            
        }

        [Fact]
        public async Task TestE2E_CreatePayment_Jane_InValidApple_Async()
        {
            var janeDoeCard = Helper.GenerateCard_JaneDoe();
            // Arrange
            var request = new
            {
                Url = "/api/merchantcardpayments",
                Body = new
                {
                    MerchantId = amazonValidMerchantGuid.ToString(),
                    RequestId = Guid.NewGuid().ToString(),
                    CardNumber = janeDoeCard.CardNumber,
                    CardHolderName = janeDoeCard.CardHolderName,
                    MonthExpiryDate = janeDoeCard.MonthExpiryDate,
                    YearExpiryDate = janeDoeCard.YearExpiryDate,
                    CVV = janeDoeCard.CVV,
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
            Assert.Equal(PaymentStatusEnum.Error.ToString(), paymentStatus.Status);
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
            Assert.Equal(PaymentStatusEnum.Error.ToString(), paymentStatus.Status);
        }
    }
}
