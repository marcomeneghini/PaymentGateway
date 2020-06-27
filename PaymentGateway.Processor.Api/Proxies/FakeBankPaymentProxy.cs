using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PaymentGateway.Processor.Api.Domain;

namespace PaymentGateway.Processor.Api.Proxies
{
    public class FakeBankPaymentProxy: IBankPaymentProxy
    {
        private readonly IMapper _mapper;

        private ConcurrentDictionary<string, CardPaymentRequest> requests =
            new ConcurrentDictionary<string, CardPaymentRequest>();

        public FakeBankPaymentProxy(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<CardPaymentResponse> CreatePaymentAsync(CardPaymentRequest request)
        {
           
            if (requests.ContainsKey(request.RequestId))
                throw new RequestIdConflictException(request.RequestId);

            // short circuit 
            if (IsRequestCardJohnDoe(request.CardHolderName, request.CardNumber, request.MonthExpiryDate, request.YearExpiryDate) ||
                IsRequestCardJaneDoe(request.CardHolderName, request.CardNumber, request.MonthExpiryDate, request.YearExpiryDate))
            {
                if (IsAppleBankAccount(request.MerchantAccountNumber, request.MerchantSortCode) ||
                    IsAmazonBankAccount(request.MerchantAccountNumber, request.MerchantSortCode))
                {
                    return new CardPaymentResponse() { RequestId = request.RequestId, TransactionId = Guid.NewGuid().ToString(), TransactionStatus = TransactionStatus.Succeeded.ToString() };
                }
            }
            throw new BankPaymentDetailsException("wrong transaction details");
        }

        public static bool IsAppleBankAccount(string accountNumber, string sortCode)
        {
            if (accountNumber == "AppleAccountNumber" &&
                sortCode == "AAPPLL")
            {
                return true;
            }

            return false;
        }

        public static bool IsAmazonBankAccount(string accountNumber, string sortCode)
        {
            if (accountNumber == "AmazonAccountNumber" &&
                sortCode == "AAMMZZ")
            {
                return true;
            }

            return false;
        }

        public static bool IsRequestCardJohnDoe(
            string cardHolderName,
            string cardNumber,
            int monthExpiryDate,
            int yearExpiryDate)
        {
            if (cardHolderName == "John Doe" &&
            cardNumber == "1111 1111 1111 1111" &&
            monthExpiryDate == 1 &&
            yearExpiryDate == 1)
            {
                return true;
            }

            return false;
        }

        public static bool IsRequestCardJaneDoe(
            string cardHolderName,
            string cardNumber,
            int monthExpiryDate,
            int yearExpiryDate)
        {
            if (cardHolderName == "Jane Doe" &&
                cardNumber == "2222 2222 2222 2222" &&
                monthExpiryDate == 2 &&
                yearExpiryDate == 2)
            {
                return true;
            }

            return false;
        }
    }
}
