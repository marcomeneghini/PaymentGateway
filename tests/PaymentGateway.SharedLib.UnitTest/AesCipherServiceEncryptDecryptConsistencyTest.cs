using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Newtonsoft.Json;
using PaymentGateway.SharedLib.Encryption;
using PaymentGateway.SharedLib.Messages;
using Xunit;

namespace PaymentGateway.SharedLib.UnitTest
{
    public class AesCipherServiceFunctionalTest
    {

        [Fact]
        public void EncryptDecryptConsistency_String()
        {
            // arrange
            AesCipherService service = new AesCipherService();
            var expectedString = "aaaasddfgsdfgsgstgsrtgstrbrthtrhrnryn";

            // act
            var encryptedString = service.Encrypt(expectedString);
            var actualString = service.Decrypt(encryptedString);

            // assert
            Assert.Equal(expectedString, actualString);
        }


    }
}
