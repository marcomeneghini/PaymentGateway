using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Domain.Entities
{
    public class Payment
    {
        public Guid PaymentId { get; set; }

        public Guid MerchantId { get; set; }

        public string RequestId { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }

        public string CardNumber { get; set; }

        /// <summary>
        /// the Card Holder Name of the customer
        /// </summary>
        public string CardHolderName { get; set; }
        /// <summary>
        /// the month of the expiry date
        /// </summary>
        public int MonthExpiryDate { get; set; }
        /// <summary>
        /// the year of the expiry date
        /// </summary>
        public int YearExpiryDate { get; set; }

        /// <summary>
        /// Card CVV
        /// </summary>
        public string CVV { get; set; }

        /// <summary>
        /// Currency of the payment
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Amount to be payed to the merchant
        /// </summary>
        public decimal Amount { get; set; }

    
        public PaymentStatus GetPaymentStatus()
        {
            return new PaymentStatus()
            {
                PaymentId = PaymentId,
                Status = PaymentStatus,
                RequestId = RequestId
            };
        }
    }


}
