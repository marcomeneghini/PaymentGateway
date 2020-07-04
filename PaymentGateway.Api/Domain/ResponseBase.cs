using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Domain
{
    public class ResponseBase
    {
        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        /// <summary>
        /// this is used to match the error sent to the Api consumer
        /// with the one more detailed in the logs
        /// </summary>
        public Guid ErrorReferencegGuid { get; set; }
    }
}
