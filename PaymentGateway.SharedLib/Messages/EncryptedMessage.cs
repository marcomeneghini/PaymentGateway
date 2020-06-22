using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PaymentGateway.SharedLib.Encryption;
using PaymentGateway.SharedLib.Exceptions;

namespace PaymentGateway.SharedLib.Messages
{
    public class EncryptedMessage
    {
        public Guid Id { get; private set; }
        /// <summary>
        /// name of the topic in the event broker
        /// where to publish the message
        /// </summary>
        public string TopicName { get; private set; }

        /// <summary>
        /// date time the message  has been pushed to the event broker
        /// </summary>
        public DateTimeOffset PushedAt { get; set; }

        /// <summary>
        /// date time the message  has been processed from a consumer
        /// </summary>
        public DateTimeOffset ProcessedAt { get; set; }

        public string ContentTypeName { get; private set; }

        public string Body { get; private set; }

        /// <summary>
        /// Source service name
        /// </summary>
        public string SourceServiceName { get; private set; }

        public EncryptedMessage(
            string topicName,
            string sourceServiceName,
            IMessage message,
            ICipherService cipherService)
        {
            Id = Guid.NewGuid();
            ContentTypeName = message.GetType().FullName;

            var serializedMessage = JsonConvert.SerializeObject(message);
            Body = cipherService.Encrypt(serializedMessage);
            SourceServiceName = sourceServiceName;
            TopicName = topicName;
        }

        public object GetMessage(ICipherService cipherService)
        {
            if (string.IsNullOrEmpty(ContentTypeName))
                throw new MessageDecryptionException(MessageDecryptionErrorReason.NoContentTypeName);
            if (string.IsNullOrEmpty(Body))
                throw new MessageDecryptionException(MessageDecryptionErrorReason.EmptyBody);
            // decrypt
            var decryptedBody = cipherService.Decrypt(Body);
            Type messageType = Type.GetType(ContentTypeName);
            var jobj = JsonConvert.DeserializeObject(decryptedBody, messageType);
            return jobj;
        }
    }
}
