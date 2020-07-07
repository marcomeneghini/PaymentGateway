using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateway.SharedLib.EventBroker
{
    public static class EventBrokerConsts
    {
        public const string PAYMENT_REQUEST_EXCHANGE_NAME = "PaymentRequests";
        public const string PAYMENT_REQUEST_ROUTINGKEY = "PaymentRequestsKey";
    }
}
