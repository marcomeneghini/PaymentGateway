using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PaymentGateway.Processor.Api.Domain;

namespace PaymentGateway.Processor.Api.Messaging
{
    public interface IChannelConsumer
    {
        Task BeginConsumeAsync(IPaymentStatusRepository paymentStatusRepository,CancellationToken cancellationToken = default);
    }
}
