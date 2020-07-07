using System;
using PaymentGateway.SharedLib.Encryption;
using PaymentGateway.SharedLib.Messages;
using Xunit;
using Moq;
using Newtonsoft.Json;
using PaymentGateway.SharedLib.Exceptions;

namespace PaymentGateway.SharedLib.UnitTest
{
    public class EncryptedMessageFunctionalTest
    {
        [Fact]
        public void VerifyBodyJsonSerialization()
        {
            // arrange
            var mockCipherService = new Mock<ICipherService>();
            mockCipherService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns<string>(x => x);
            var paymentRequestMessage =  createFakePaymentRequestMessage();
            var expectedBody  = JsonConvert.SerializeObject(paymentRequestMessage);

            // act
            var encryptedMessage = new EncryptedMessage("", "", "", paymentRequestMessage, mockCipherService.Object);

            // assert
            Assert.Equal(expectedBody, encryptedMessage.Body);
        }

        [Fact]
        public void VerifyGetMessageResult()
        {
            // arrange
            var mockCipherService = new Mock<ICipherService>();
            mockCipherService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns<string>(x => x);
            mockCipherService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns<string>(x => x);
            var expectedMessage = createFakePaymentRequestMessage();
           

            // act
            var encryptedMessage = new EncryptedMessage("", "", "", expectedMessage, mockCipherService.Object);
            var actualResult= encryptedMessage.GetMessage<PaymentRequestMessage>(mockCipherService.Object);
            // assert
            Assert.Equal(expectedMessage.RequestId, actualResult.RequestId);
            Assert.Equal(expectedMessage.CardNumber, actualResult.CardNumber);
            Assert.Equal(expectedMessage.CardHolderName, actualResult.CardHolderName);
            Assert.Equal(expectedMessage.MonthExpiryDate, actualResult.MonthExpiryDate);
            Assert.Equal(expectedMessage.YearExpiryDate, actualResult.YearExpiryDate);
            Assert.Equal(expectedMessage.Currency, actualResult.Currency);
            Assert.Equal(expectedMessage.CVV, actualResult.CVV);
            Assert.Equal(expectedMessage.MerchantSortCode, actualResult.MerchantSortCode);
            Assert.Equal(expectedMessage.MerchantAccountNumber, actualResult.MerchantAccountNumber);
        }

        [Fact]
        public void VerifyGetMessageResult_EmptyContentTypeNameChanged()
        {
            // arrange
            var mockCipherService = new Mock<ICipherService>();
            mockCipherService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns<string>(x => x);
            mockCipherService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns<string>(x => x);
            var expectedMessage = createFakePaymentRequestMessage();


            // act
            var encryptedMessage = new EncryptedMessage("", "", "", expectedMessage, mockCipherService.Object);
            encryptedMessage.ContentTypeName = "";

            // act/assert
            var exception=  Assert.Throws<MessageDecryptionException>(() => encryptedMessage.GetMessage<PaymentRequestMessage>(mockCipherService.Object));
            Assert.Equal(MessageDecryptionErrorReason.NoContentTypeName, exception.MessageDecryptionErrorReason);

        }

        [Fact]
        public void VerifyGetMessageResult_ChangedContentTypeNameChanged()
        {
            // arrange
            var mockCipherService = new Mock<ICipherService>();
            mockCipherService.Setup(x => x.Encrypt(It.IsAny<string>())).Returns<string>(x => x);
            mockCipherService.Setup(x => x.Decrypt(It.IsAny<string>())).Returns<string>(x => x);
            var expectedMessage = createFakePaymentRequestMessage();


            // act
            var encryptedMessage = new EncryptedMessage("", "", "", expectedMessage, mockCipherService.Object);
            encryptedMessage.ContentTypeName += "sss";

            // act/assert
            var exception = Assert.Throws<MessageDecryptionException>(() => encryptedMessage.GetMessage<PaymentRequestMessage>(mockCipherService.Object));
            Assert.Equal(MessageDecryptionErrorReason.WrongContentType, exception.MessageDecryptionErrorReason);

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
