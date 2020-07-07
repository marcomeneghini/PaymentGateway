using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Newtonsoft.Json;
using PaymentGateway.SharedLib.Encryption;
using PaymentGateway.SharedLib.Exceptions;

namespace PaymentGateway.SharedLib.Messages
{
    public class EncryptedMessage
    {
        public Guid Id { get;  set; }
        /// <summary>
        /// name of the topic in the event broker
        /// where to publish the message
        /// </summary>
        public string TopicName { get; set; }

        public string RoutingKey { get; set; }

        /// <summary>
        /// date time the message  has been pushed to the event broker
        /// </summary>
        public DateTimeOffset PushedAt { get; set; }

        /// <summary>
        /// date time the message  has been processed from a consumer
        /// </summary>
        public DateTimeOffset ProcessedAt { get; set; }

        public string ContentTypeName { get; set; }

        public string Body { get;  set; }

        /// <summary>
        /// Source service name
        /// </summary>
        public string SourceServiceName { get; set; }

        /// <summary>
        /// this constructor is necessary for the deserializing process
        /// so ContentTypeName and Body can be set externally after object constructor 
        /// </summary>
        public EncryptedMessage()
        {
                
        }
        public EncryptedMessage(
            string topicName,
            string routingKey,
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
            RoutingKey = routingKey;
        }

        public T GetMessage<T>(ICipherService cipherService)
        {
            if (string.IsNullOrEmpty(ContentTypeName))
                throw new MessageDecryptionException(MessageDecryptionErrorReason.NoContentTypeName);
            if (string.IsNullOrEmpty(Body))
                throw new MessageDecryptionException(MessageDecryptionErrorReason.EmptyBody);
            // decrypt
            var decryptedBody = cipherService.Decrypt(Body);
            try
            {
                Type messageType = Type.GetType(ContentTypeName);
                dynamic jobj = JsonConvert.DeserializeObject(decryptedBody, messageType);
                return jobj;
            }
            catch (Exception e)
            {
                throw new MessageDecryptionException(MessageDecryptionErrorReason.WrongContentType);
            }
           
        }
    }
}
