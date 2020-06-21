using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Bank.Payments.Api.Domain;

namespace Bank.Payments.Api.Services
{
    public class CardPaymentService:ICardPaymentService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IBankAccountRepository _bankAccountRepository;

        public CardPaymentService(ICardRepository cardRepository, IBankAccountRepository bankAccountRepository)
        {
            _cardRepository = cardRepository;
            _bankAccountRepository = bankAccountRepository;
        }

        public CardPaymentResponse DoPayment(CardPaymentRequest request)
        {
            var cards = _cardRepository.GetAllCards();
            if (!cards.Contains(request.GetCard()))
                return new CardPaymentResponse { TransactionStatus = TransactionStatus.Declined, Message = "Card not present", RequestId = request.RequestId };
            
            var  bankAccounts = _bankAccountRepository.GetAllBankAccounts();
            if (!bankAccounts.Contains(request.GetBankAccount()))
                return new CardPaymentResponse { TransactionStatus = TransactionStatus.Declined, Message = "Bank account not present", RequestId = request.RequestId };
            
            return new CardPaymentResponse { TransactionStatus = TransactionStatus.Succeeded, RequestId = request.RequestId };

        }


        private CardPaymentRequest Create_Card1_PaymentRequest()
        {
            return new CardPaymentRequest()
            {
                Amount = 10,
                CVV = "000",
                CardHolderName = "Holder Card1 FullName",
                CardNumber = "1234 1234 1234 1234",
                Currency = "GBP",
                MerchantAccountNumber = "12345678",
                MerchantSortCode = "112233",
                MonthExpiryDate = 1,
                YearExpiryDate = 2021
            };
        }
    }
}
