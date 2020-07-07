using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.Processor.Api.Messaging;
using PaymentGateway.SharedLib.Encryption;
using PaymentGateway.SharedLib.Messages;
using Xunit;

namespace PaymentGateway.Processor.Api.UnitTest
{
    public class ChannelProducerTest
    {
        //IPaymentStatusRepository paymentStatusRepository,
        //    ICipherService cipherService,
        //ILogger<ChannelProducer> logger)

        [Fact]
        public async Task ShouldProduceEncryptedMessage()
        {
            var channel = Channel.CreateUnbounded<EncryptedMessage>();
            var mockPaymentStatusRepository =new Mock<IPaymentStatusRepository>();
            var mockCipherService = new Mock<ICipherService>();
            var mockLogger = new Mock<ILogger<ChannelProducer>>();

            // cipher pass through no encryption 
            mockCipherService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns<string>(x => x);
            mockCipherService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns<string>(x => x);

            ChannelProducer producer=new ChannelProducer(channel.Writer,  mockCipherService.Object, mockLogger.Object);

            var paymentRequest = createFakePaymentRequestMessage();
            EncryptedMessage msg =new EncryptedMessage("topic","routingkey","sourceServiceName", paymentRequest, mockCipherService.Object);

            await producer.PublishAsync(mockPaymentStatusRepository.Object, msg);

            await foreach (var message in channel.Reader.ReadAllAsync())
            {
                Assert.Equal(msg.Body,message.Body);
                Assert.Equal(msg.TopicName, message.TopicName);
                Assert.Equal(msg.RoutingKey, message.RoutingKey);
                Assert.Equal(msg.PushedAt, message.PushedAt);
                Assert.Equal(msg.ContentTypeName, message.ContentTypeName);
                break;
            }
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
