using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Client.Payments.Api.Utils;


namespace Client.Payments.Api.Models
{
    public class PaymentModel
    {
        public string RequestId { get; set; }
        /// <summary>
        /// the card number of the customer
        /// </summary>
        [Required]
        [RegularExpression(RegexValidator.VALID_CARD_NUMBER)]
        public string CardNumber { get; set; }

        /// <summary>
        /// the Card Holder Name of the customer
        /// </summary>
        [Required]
        public string CardHolderName { get; set; }
        /// <summary>
        /// the month of the expiry date
        /// </summary>
        [Range(1, 12)]
        public int MonthExpiryDate { get; set; }

        /// <summary>
        /// the year of the expiry date
        /// </summary>
        [Range(2020, 2025)]
        public int YearExpiryDate { get; set; }

        /// <summary>
        /// Card CVV
        /// </summary>
        [Required]
        [RegularExpression(RegexValidator.VALID_CARD_CCV)]
        public string CVV { get; set; }

        /// <summary>
        /// Currency of the payment
        /// </summary>
        [Required]
        public string Currency { get; set; }

        /// <summary>
        /// Amount to be payed to the merchant
        /// </summary>
        [Required]
        public decimal Amount { get; set; }
    }
}
