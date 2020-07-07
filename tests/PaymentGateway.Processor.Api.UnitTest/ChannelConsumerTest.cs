using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.Processor.Api.Domain.Entities;
using PaymentGateway.Processor.Api.MappingProfiles;
using PaymentGateway.Processor.Api.Messaging;
using PaymentGateway.SharedLib.Encryption;
using PaymentGateway.SharedLib.Messages;
using Xunit;

namespace PaymentGateway.Processor.Api.UnitTest
{
    public class ChannelConsumerTest
    {
        //ChannelReader<EncryptedMessage> reader,
        //    ILogger<ChannelConsumer> logger,
        //IPaymentStatusRepository paymentStatusRepository,
        //    IBankPaymentProxy bankPaymentProxy,
        //ICipherService cipherService,
        //    IMapper mapper)
        [Fact]
        public async Task ShouldUpdatePaymentStatusToError()
        {
            var channel = Channel.CreateUnbounded<EncryptedMessage>();
            var mockPaymentStatusRepository = new Mock<IPaymentStatusRepository>();
            var mockBankPaymentProxy = new Mock<IBankPaymentProxy>();
            var mockCipherService = new Mock<ICipherService>();
            var mockLogger = new Mock<ILogger<ChannelConsumer>>();
            // create a mapper from original configuration/profile
            var mapConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DomainProfile());
            });
            var mapper = mapConfig.CreateMapper();
            // cipher pass through no encryption
            mockCipherService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns<string>(x => x);
            mockCipherService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns<string>(x => x);
            
            var paymentRequest = createFakePaymentRequestMessage();
            EncryptedMessage msg = new EncryptedMessage("topic", "routingkey", "sourceServiceName", paymentRequest, mockCipherService.Object);
            var paymentStatus = new PaymentStatus()
            {
                RequestId = paymentRequest.RequestId,
                Status = PaymentStatusEnum.Scheduled.ToString()
            };
            
            // setup the paymentRepository
             mockPaymentStatusRepository.Setup(x => x.GetPaymentStatus(paymentRequest.PaymentRequestId))
                .ReturnsAsync(paymentStatus);
            //_paymentStatusRepository.GetPaymentStatus(decryptedMessage.PaymentRequestId);

            var channelConsumer =new ChannelConsumer(
                channel.Reader, mockLogger.Object, 
               
                mockBankPaymentProxy.Object,
                mockCipherService.Object,
                mapper);
            // push a message in the channel
            await channel.Writer.WriteAsync(msg);
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(3));
            await channelConsumer.BeginConsumeAsync(mockPaymentStatusRepository.Object, cts.Token);

           
            Assert.Equal(PaymentStatusEnum.Error.ToString(), paymentStatus.Status);

        }

        [Fact]
        public async Task ShouldUpdatePaymentStatusToCompleted()
        {
            var channel = Channel.CreateUnbounded<EncryptedMessage>();
            var mockPaymentStatusRepository = new Mock<IPaymentStatusRepository>();
            var mockBankPaymentProxy = new Mock<IBankPaymentProxy>();
            var mockCipherService = new Mock<ICipherService>();
            var mockLogger = new Mock<ILogger<ChannelConsumer>>();
            // create a mapper from original configuration/profile
            var mapConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DomainProfile());
            });
            var mapper = mapConfig.CreateMapper();
            // cipher pass through no encryption
            mockCipherService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns<string>(x => x);
            mockCipherService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns<string>(x => x);

            var paymentRequest = createFakePaymentRequestMessage();
            EncryptedMessage msg = new EncryptedMessage("topic", "routingkey", "sourceServiceName", paymentRequest, mockCipherService.Object);
            var paymentStatus = new PaymentStatus()
            {
                RequestId = paymentRequest.RequestId,
                Status = PaymentStatusEnum.Scheduled.ToString()
            };

            // setup the paymentRepository
            mockPaymentStatusRepository.Setup(x => x.GetPaymentStatus(paymentRequest.PaymentRequestId))
               .ReturnsAsync(paymentStatus);
            var cardPaymentRequest = mapper.Map<CardPayment>(paymentRequest);
            var cardPaymentResponse = new PaymentResult()
            {
                RequestId = paymentRequest .RequestId,
                TransactionStatus = TransactionStatus.Succeeded.ToString()
            };
            mockBankPaymentProxy.Setup(x => x.CreatePaymentAsync(It.IsAny<CardPayment>())).ReturnsAsync(cardPaymentResponse);

           
            var channelConsumer = new ChannelConsumer(
                channel.Reader, mockLogger.Object,
               
                mockBankPaymentProxy.Object,
                mockCipherService.Object,
                mapper);
            // push a message in the channel
            await channel.Writer.WriteAsync(msg);
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(3));
            await channelConsumer.BeginConsumeAsync(mockPaymentStatusRepository.Object, cts.Token);


            Assert.Equal(PaymentStatusEnum.Completed.ToString(), paymentStatus.Status);

        }

        private static PaymentRequestMessage createFakePaymentRequestMessage()
        {
            return new PaymentRequestMessage()
            {
                Amount = 11,
                CardNumber = "CardNumber",
                RequestId = "RequestId",
                CardHolderName = "CardHolderName",
                MonthExpiryDate = 1,
                YearExpiryDate = 2,
                Currency = "Currency",
                CVV = "CVV",
                MerchantSortCode = "MerchantSortCode",
                MerchantAccountNumber = "MerchantAccountNumber",
                PaymentRequestId = Guid.NewGuid()

            };
        }
    }
}
