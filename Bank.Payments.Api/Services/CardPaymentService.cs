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
        private readonly IPaymentRepository _paymentRepository;


        public CardPaymentService(ICardRepository cardRepository, IBankAccountRepository bankAccountRepository,IPaymentRepository paymentRepository)
        {
            _cardRepository = cardRepository;
            _bankAccountRepository = bankAccountRepository;
            _paymentRepository = paymentRepository;
        }

        public CardPaymentResponse DoPayment(CardPaymentRequest request)
        {
            var paymentStatus = _paymentRepository.GetPaymentStatus(request.RequestId);
            if (paymentStatus != null)
                throw new RequestAlreadyProcessedException(paymentStatus.Status, paymentStatus.RequestId);
            var currentPaymentStatus = new PaymentStatus()
                {RequestId = request.RequestId, Status = PaymentStatusEnum.Scheduled};
            _paymentRepository.AddPaymentStatus(currentPaymentStatus);

            var cards = _cardRepository.GetAllCards();
            if (!cards.Contains(request.GetCard()))
                return new CardPaymentResponse { TransactionStatus = TransactionStatus.Declined, Message = "Card not present", RequestId = request.RequestId };
            
            var  bankAccounts = _bankAccountRepository.GetAllBankAccounts();
            if (!bankAccounts.Contains(request.GetBankAccount()))
                return new CardPaymentResponse { TransactionStatus = TransactionStatus.Declined, Message = "Bank account not present", RequestId = request.RequestId };
            
            // perform the transaction
            try
            {
                // perform the transaction

                // transaction ok
                currentPaymentStatus.Status = PaymentStatusEnum.Completed;
                _paymentRepository.UpdatePaymentStatus(currentPaymentStatus);
            }
            catch (Exception e)
            {
                currentPaymentStatus.Status = PaymentStatusEnum.Error;
                _paymentRepository.UpdatePaymentStatus(currentPaymentStatus);
                throw;
            }
            // return the status
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
