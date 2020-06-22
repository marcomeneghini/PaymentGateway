using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using PaymentGateway.Api.Services;
using Xunit;
using Moq;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Domain.Exceptions;

namespace PaymentGateway.Api.UnitTest
{
    public class PaymentServiceTest
    {
       
        [Fact]
        public async Task CreatePayment_OperationDuplicated()
        {
            // arrange 
            Guid fakePaymentGuid = Guid.NewGuid();
            string fakePaymentRequestId = "asdasdad";
            var mockMerchantRepository = new Mock<IMerchantRepository>();
            var mockPaymentRepository = new Mock<IPaymentRepository>();
            var fakePaymentStatus = CreateFakePaymentStatus(fakePaymentGuid, fakePaymentRequestId);
            mockPaymentRepository.Setup(x => x.GetPaymentStatus(fakePaymentRequestId))
                .ReturnsAsync(fakePaymentStatus);
            var request = CreateFakeCreatePaymentRequest_ForDuplicateTest(fakePaymentStatus);
            // act
            var service = new PaymentService(mockMerchantRepository.Object, mockPaymentRepository.Object);
            // act-assert
            var exception = await Assert.ThrowsAsync<RequestAlreadyProcessedException>(() =>  service.CreatePayment(request));
            Assert.Equal(fakePaymentRequestId, exception.RequestId);
          
        }

        [Fact]
        public async Task CreatePayment_MerchantNotPresent()
        {

            // arrange 
            var mockMerchantRepository = new Mock<IMerchantRepository>();
            var mockPaymentRepository = new Mock<IPaymentRepository>();
            var request = CreateFakeCreatePaymentRequest_ForInvalidMerchant(Guid.NewGuid());
            // act
            var service = new PaymentService(mockMerchantRepository.Object, mockPaymentRepository.Object);
            // act-assert
            var exception = await Assert.ThrowsAsync<InvalidMerchantException>(() => service.CreatePayment(request));
            Assert.Equal(InvalidMerchantReason.NotPresent, exception.InvalidMerchantReason);
        }
        [Fact]
        public async Task CreatePayment_MerchantNotValid()
        {

            // arrange 
            var mockMerchantRepository = new Mock<IMerchantRepository>();
            var mockPaymentRepository = new Mock<IPaymentRepository>();
            Guid merchantId = Guid.NewGuid();
            var request = CreateFakeCreatePaymentRequest_ForInvalidMerchant(merchantId);
            var merchant = createFakeInvalidMerchant(merchantId);
            mockMerchantRepository.Setup(x => x.GetMerchantById(merchantId)).ReturnsAsync(merchant);
            // act
            var service = new PaymentService(mockMerchantRepository.Object, mockPaymentRepository.Object);
            // act-assert
            var exception = await Assert.ThrowsAsync<InvalidMerchantException>(() => service.CreatePayment(request));
            Assert.Equal(InvalidMerchantReason.Invalid, exception.InvalidMerchantReason);
        }

        private PaymentStatus CreateFakePaymentStatus(Guid id, string requestId)
        {
            return  new PaymentStatus()
            {
                PaymentId = id,
                Status = PaymentStatusEnum.Scheduled,
                RequestId = requestId
            };
        }

        private CreatePaymentRequest CreateFakeCreatePaymentRequest_ForDuplicateTest(PaymentStatus paymentStatus)
        {
            return new CreatePaymentRequest
            {
                RequestId = paymentStatus.RequestId,
                
            };

        }
        private CreatePaymentRequest CreateFakeCreatePaymentRequest_ForInvalidMerchant(Guid merchantId)
        {
            return new CreatePaymentRequest
            {
                MerchantId = merchantId,
                RequestId = "sdsdfsdfsdf",

            };

        }

        private Merchant createFakeInvalidMerchant(Guid merchantId)
        {
            return new Merchant()
            {
                IsValid = false,
                Id = merchantId
            };

        }
    }
}
