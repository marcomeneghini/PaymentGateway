using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using PaymentGateway.Api.Services;
using Xunit;
using Moq;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Domain.Entities;
using PaymentGateway.Api.Domain.Exceptions;
using PaymentGateway.SharedLib.Encryption;
using PaymentGateway.SharedLib.EventBroker;

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
            var mockCipherService = new Mock<ICipherService>();
            var mockPaymentRepository = new Mock<IPaymentRepository>();
            var mockEventBrokerPublisher = new Mock<IEventBrokerPublisher>();
            var mockLogger = new Mock<ILogger<PaymentService>>();
            var fakePaymentStatus = CreateFakePaymentStatus(fakePaymentGuid, fakePaymentRequestId);
            mockPaymentRepository.Setup(x => x.GetPaymentStatus(fakePaymentRequestId))
                .ReturnsAsync(fakePaymentStatus);
            var request = CreateFakeCreatePayment_ForDuplicateTest(fakePaymentStatus);
            // act
            var service = new PaymentService(
                mockMerchantRepository.Object,
                mockPaymentRepository.Object, 
                mockEventBrokerPublisher.Object,
                mockCipherService.Object,
                mockLogger.Object);

            // ACT
            var response = await service.CreatePayment(request);

            // ASSERT
            Assert.Equal(Consts.DUPLICATE_REQUEST_CODE, response.ErrorCode);

        }

        [Fact]
        public async Task CreatePayment_MerchantNotPresent()
        {

            // arrange 
            var mockMerchantRepository = new Mock<IMerchantRepository>();
            var mockPaymentRepository = new Mock<IPaymentRepository>();
            var mockCipherService = new Mock<ICipherService>();
            var mockEventBrokerPublisher = new Mock<IEventBrokerPublisher>();
            var mockLogger = new Mock<ILogger<PaymentService>>();
            var request = CreateFakeCreatePayment_ForInvalidMerchant(Guid.NewGuid());
            // act
            var service = new PaymentService(
                mockMerchantRepository.Object,
                mockPaymentRepository.Object, 
                mockEventBrokerPublisher.Object, 
                mockCipherService.Object,
                mockLogger.Object);
            

            // ACT
            var response = await service.CreatePayment(request);

            // ASSERT
            Assert.Equal(Consts.MERCHANT_NOT_PRESENT_CODE, response.ErrorCode);
        }
        [Fact]
        public async Task CreatePayment_MerchantNotValid()
        {
            // arrange 
            var mockMerchantRepository = new Mock<IMerchantRepository>();
            var mockPaymentRepository = new Mock<IPaymentRepository>();
            var mockCipherService = new Mock<ICipherService>();
            var mockEventBrokerPublisher = new Mock<IEventBrokerPublisher>();
            var mockLogger = new Mock<ILogger<PaymentService>>();
            Guid merchantId = Guid.NewGuid();
            var request = CreateFakeCreatePayment_ForInvalidMerchant(merchantId);
            var merchant = createFakeInvalidMerchant(merchantId);
            mockMerchantRepository.Setup(x => x.GetMerchantById(merchantId)).ReturnsAsync(merchant);
            // act
            var service = new PaymentService(
                mockMerchantRepository.Object,
                mockPaymentRepository.Object, 
                mockEventBrokerPublisher.Object, 
                mockCipherService.Object,
                mockLogger.Object);
            // act-assert
            //var exception = await Assert.ThrowsAsync<InvalidMerchantException>(() => service.CreatePayment(request));
            //Assert.Equal(InvalidMerchantReason.Invalid, exception.InvalidMerchantReason);

            // ACT
            var response = await service.CreatePayment(request);

            // ASSERT
            Assert.Equal(Consts.MERCHANT_INVALID_CODE, response.ErrorCode);

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

        private Payment CreateFakeCreatePayment_ForDuplicateTest(PaymentStatus paymentStatus)
        {
            return new Payment
            {
                RequestId = paymentStatus.RequestId,
                
            };
        }

        private Payment CreateFakeCreatePayment_ForInvalidMerchant(Guid merchantId)
        {
            return new Payment
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
