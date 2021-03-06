﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Payments.Api.Domain.Entities
{
    public class Payment
    {
        public string RequestId { get; set; }

        public Guid MerchantId { get; set; }

        /// <summary>
        /// the card number of the customer
        /// </summary>
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
    }
}
