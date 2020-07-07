using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PaymentGateway.Processor.Api.Models;
using Xunit;

namespace PaymentGateway.Processor.Api.IntegrationTests
{
    public class PaymentStatusesTests : IClassFixture<TestFixture<TestStartupNoAuth>>
    {
        private HttpClient Client;
        private Guid notPresentPaymentId= Guid.NewGuid();

        private Guid paymentStatusScheduledId = new Guid("55577777-4444-447C-ABC5-0AF6CF829A22");
        private Guid paymentStatusCompletedId = new Guid("66677777-4444-447C-ABC5-0AF6CF829A22");
        private Guid paymentStatusErrorId = new Guid("77777777-4444-447C-ABC5-0AF6CF829A22");
        public PaymentStatusesTests(TestFixture<TestStartupNoAuth> fixture)
        {
            Client = fixture.Client;
        }

        [Fact]
        public async Task TestGetPaymentStatus_NotFound_Async()
        {
           
            // Arrange
            var request = $"/api/PaymentStatuses?paymentId={notPresentPaymentId}";

            // Act
            var response = await Client.GetAsync(request);

            // Assert
         
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            
        }


        [Fact]
        public async Task TestGetPaymentStatus_Scheduled_Async()
        {

            // Arrange
            var request = $"/api/PaymentStatuses?paymentId={paymentStatusScheduledId}";

            // Act
            var response = await Client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            
            var stringResponse = await response.Content.ReadAsStringAsync();
            var paymentStatus = JsonConvert.DeserializeObject<PaymentStatusModel>(stringResponse);
            Assert.Equal(paymentStatus.Status, PaymentStatusEnum.Scheduled.ToString());
            
        }

        [Fact]
        public async Task TestGetPaymentStatus_Completed_Async()
        {

            // Arrange
            var request = $"/api/PaymentStatuses?paymentId={paymentStatusCompletedId}";

            // Act
            var response = await Client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var paymentStatus = JsonConvert.DeserializeObject<PaymentStatusModel>(stringResponse);
            Assert.Equal(paymentStatus.Status, PaymentStatusEnum.Completed.ToString());
        }

        [Fact]
        public async Task TestGetPaymentStatus_Error_Async()
        {

            // Arrange
            var request = $"/api/PaymentStatuses?paymentId={paymentStatusErrorId}";

            // Act
            var response = await Client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var paymentStatus = JsonConvert.DeserializeObject<PaymentStatusModel>(stringResponse);
            Assert.Equal(paymentStatus.Status, PaymentStatusEnum.Error.ToString());
        }


    }
}
