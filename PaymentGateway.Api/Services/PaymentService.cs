using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Domain.Exceptions;
using PaymentGateway.SharedLib.Encryption;
using PaymentGateway.SharedLib.EventBroker;
using PaymentGateway.SharedLib.Messages;

namespace PaymentGateway.Api.Services
{
    public class PaymentService:IPaymentService
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IEventBrokerPublisher _eventBrokerPublisher;
        private readonly ICipherService _cipherService;

        public PaymentService(
            IMerchantRepository merchantRepository,
            IPaymentRepository paymentRepository,
            IEventBrokerPublisher eventBrokerPublisher,
            ICipherService cipherService)
        {
            _merchantRepository = merchantRepository;
            _paymentRepository = paymentRepository;
            _eventBrokerPublisher = eventBrokerPublisher;
            _cipherService = cipherService;
        }
        public async Task<CreatePaymentResponse> CreatePayment(CreatePaymentRequest request)
        {
            // verify if the payment request exists already
            var paymentStatus = await _paymentRepository.GetPaymentStatus(request.RequestId);
            if (paymentStatus != null)
                throw new RequestAlreadyProcessedException(paymentStatus.Status, paymentStatus.RequestId);
            paymentStatus= new PaymentStatus()
                { PaymentId = Guid.NewGuid(), RequestId = request.RequestId, Status = PaymentStatusEnum.Scheduled };
            await _paymentRepository.AddPaymentStatus(paymentStatus);
            try
            {

                // get the merchant and check
                var merchant = await _merchantRepository.GetMerchantById(request.MerchantId);
                if (merchant == null)
                    throw new InvalidMerchantException(request.MerchantId, InvalidMerchantReason.NotPresent);
                if (!merchant.IsValid)
                    throw new InvalidMerchantException(merchant.Id, InvalidMerchantReason.Invalid);

                // Prepare the full payment data to send to the Event Broker
              
                var paymentRequestMessage = createPaymentRequestMessage(request, merchant);
                paymentRequestMessage.PaymentRequestId = paymentStatus.PaymentId;

                var encryptedMessage=new EncryptedMessage(EventBrokerConsts.PAYMENT_REQUEST_EXCHANGE_NAME, "PaymentGateway.Api", paymentRequestMessage, _cipherService);
                _eventBrokerPublisher.PublishEncryptedMessage(encryptedMessage);

                    // set the payment status
                paymentStatus.Status = PaymentStatusEnum.Completed;
                await  _paymentRepository.UpdatePaymentStatus(paymentStatus);
                //TODO: set the correct response
                return new CreatePaymentResponse(){RequestId = request.RequestId,PaymentRequestId = paymentStatus.PaymentId};
            }
            catch (Exception e)
            {
                paymentStatus.Status = PaymentStatusEnum.Error;
                await _paymentRepository.UpdatePaymentStatus(paymentStatus);
                throw;
            }
        }

        private PaymentRequestMessage createPaymentRequestMessage(CreatePaymentRequest paymentRequest,Merchant merchant)
        {
            return new PaymentRequestMessage()
            {
                Amount = paymentRequest.Amount,
                CVV = paymentRequest.CVV,
                CardHolderName = paymentRequest.CardHolderName,
                CardNumber = paymentRequest.CardNumber,
                Currency = paymentRequest.Currency,
                MerchantAccountNumber = merchant.AccountNumber,
                MerchantSortCode = merchant.SortCode,
                MonthExpiryDate = paymentRequest.MonthExpiryDate,
                YearExpiryDate = paymentRequest.YearExpiryDate,
                RequestId = paymentRequest.RequestId
            };
        }
    }
}
