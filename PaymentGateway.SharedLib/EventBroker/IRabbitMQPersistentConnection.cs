using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace PaymentGateway.SharedLib.EventBroker
{
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
