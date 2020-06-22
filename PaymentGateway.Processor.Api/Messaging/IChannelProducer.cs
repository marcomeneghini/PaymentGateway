using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PaymentGateway.SharedLib.Messages;

namespace PaymentGateway.Processor.Api.Messaging
{
    public interface IChannelProducer
    {
        Task PublishAsync(EncryptedMessage message, CancellationToken cancellationToken = default);
    }
}
