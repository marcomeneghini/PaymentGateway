using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace Bank.Payments.Api.Domain
{
    public class CardPaymentRequest
    {
        public CardPaymentRequest()
        {
                
        }

        public CardPaymentRequest(Card card,BankAccount bankAccount)
        {
            CardHolderName = card.CardHolderName;
            MonthExpiryDate = card.MonthExpiryDate;
            YearExpiryDate = card.YearExpiryDate;
            CardNumber = card.CardNumber;

            MerchantAccountNumber = bankAccount.AccountNumber;
            MerchantSortCode = bankAccount.SortCode;
        }

        public CardPaymentRequest(string requestId,Card card, BankAccount bankAccount):this(card,bankAccount)
        {
            RequestId = requestId;
        }



        public string RequestId { get; set; }
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

        /// <summary>
        /// Merchant Sort Code
        /// </summary>
        public string MerchantSortCode { get; set; }

        /// <summary>
        /// Merchant Bank Account
        /// </summary>
        public string MerchantAccountNumber { get; set; }

        public Card GetCard()
        {
            return new Card()
            {
                CardHolderName = CardHolderName,
                MonthExpiryDate = MonthExpiryDate,
                YearExpiryDate = YearExpiryDate,
                CardNumber = CardNumber

            };
        }

        public BankAccount GetBankAccount()
        {
            return new BankAccount()
            {
                AccountNumber = MerchantAccountNumber,
                SortCode = MerchantSortCode
            };
        }

    }
}
