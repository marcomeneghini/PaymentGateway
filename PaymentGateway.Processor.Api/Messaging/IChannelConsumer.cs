using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Processor.Api.Messaging
{
    public interface IChannelConsumer
    {
        Task BeginConsumeAsync(CancellationToken cancellationToken = default);
    }
}
