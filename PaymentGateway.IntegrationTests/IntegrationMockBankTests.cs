using System;
using System.Collections.Generic;
using System.Net;
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
    public  class IntegrationMockBankTests : IClassFixture<IntegrationMockBankTestFixture<PgApi.TestStartupNoAuth, PgProcessorApi.TestStartupNoAuth>>
    {
        private HttpClient PgApiClient;
        private HttpClient PgProcApiClient;

        private Guid unknownMerchantGuid = new Guid("00092C77-3C0E-447C-ABC5-0AF6CF829A22");
       
        private Guid amazonValidMerchantGuid = InMemoryMerchantRepository.CreateMerchant_Amazon().Id;
        private Guid appleInValidMerchantGuid = InMemoryMerchantRepository.CreateMerchant_InvalidApple().Id;


        public IntegrationMockBankTests(IntegrationMockBankTestFixture<PgApi.TestStartupNoAuth, PgProcessorApi.TestStartupNoAuth> fixture)
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
                    Currency = "GBP",
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
                await Task.Delay(500);
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
                        if (paymentStatus.Status== PaymentStatusEnum.Scheduled.ToString())
                        {
                            await Task.Delay(1000);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            Assert.Equal(PaymentStatusEnum.Completed.ToString(),paymentStatus.Status);
            // this to check the consistency of the distributed operation
            Assert.Equal(merchantPaymentResponse.RequestId, paymentStatus.RequestId);
        }

        [Fact]
        public async Task TestE2E_CreatePayment_Jane_ValidAmazon_Async()
        {
            try
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
                        Currency="GBP",
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
                    await Task.Delay(500);
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
                            if (paymentStatus.Status == PaymentStatusEnum.Scheduled.ToString())
                            {
                                await Task.Delay(1000);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                Assert.Equal(PaymentStatusEnum.Completed.ToString(), paymentStatus.Status);
                // this to check the consistency of the distributed operation
                Assert.Equal(merchantPaymentResponse.RequestId, paymentStatus.RequestId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
                    CardNumber = "1298 0000 0000 0000",
                    CardHolderName = "Luke Doe",
                    MonthExpiryDate = 10,
                    YearExpiryDate = 2022,
                    CVV = "000",
                    Currency = "GBP",
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
                await Task.Delay(500);
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
                        if (paymentStatus.Status == PaymentStatusEnum.Scheduled.ToString())
                        {
                            await Task.Delay(1000);
                        }
                        else
                        {
                            break;
                        }
                    }
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
