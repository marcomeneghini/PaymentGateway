using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Domain.Entities;
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
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IMerchantRepository merchantRepository,
            IPaymentRepository paymentRepository,
            IEventBrokerPublisher eventBrokerPublisher,
            ICipherService cipherService,
            ILogger<PaymentService> logger
            )
        {
            _merchantRepository = merchantRepository;
            _paymentRepository = paymentRepository;
            _eventBrokerPublisher = eventBrokerPublisher;
            _cipherService = cipherService;
            _logger = logger;
        }
        public async Task<CreatePaymentResponse> CreatePayment(Payment payment)
        {
            // verify if the payment request exists already
            var paymentStatus = await _paymentRepository.GetPaymentStatus(payment.RequestId);
            if (paymentStatus != null)
                return new CreatePaymentResponse { ErrorCode = Consts.DUPLICATE_REQUEST_CODE };
            
             
            paymentStatus= new PaymentStatus()
                { PaymentId = Guid.NewGuid(), RequestId = payment.RequestId, Status = PaymentStatusEnum.Scheduled };
            await _paymentRepository.AddPaymentStatus(paymentStatus);
            try
            {

                // get the merchant and check
                var merchant = await _merchantRepository.GetMerchantById(payment.MerchantId);
                if (merchant == null)
                    return new CreatePaymentResponse { ErrorCode = Consts.MERCHANT_NOT_PRESENT_CODE };
                if (!merchant.IsValid)
                    return new CreatePaymentResponse { ErrorCode = Consts.MERCHANT_INVALID_CODE };

                // Prepare the full payment data to send to the Event Broker

                var paymentRequestMessage = createPaymentRequestMessage(payment, merchant);
                paymentRequestMessage.PaymentRequestId = paymentStatus.PaymentId;

                var encryptedMessage=new EncryptedMessage(EventBrokerConsts.PAYMENT_REQUEST_EXCHANGE_NAME, EventBrokerConsts.PAYMENT_REQUEST_ROUTINGKEY, "PaymentGateway.Api", paymentRequestMessage, _cipherService);
                _eventBrokerPublisher.PublishEncryptedMessage(encryptedMessage);

                    // set the payment status
                paymentStatus.Status = PaymentStatusEnum.Completed;
                await  _paymentRepository.UpdatePaymentStatus(paymentStatus);
                // create the response
                return new CreatePaymentResponse(){RequestId = payment.RequestId,PaymentRequestId = paymentStatus.PaymentId};
            }
            catch (Exception e)
            {
                var errorReferenceGuid = Guid.NewGuid();
                _logger.LogError($"ReferenceId:{errorReferenceGuid}, Error Message:{e.Message}");
                paymentStatus.Status = PaymentStatusEnum.Error;
                await _paymentRepository.UpdatePaymentStatus(paymentStatus);

                return new CreatePaymentResponse { ErrorCode = Consts.UNEXPECTED_ERROR_CODE, ErrorReferencegGuid = errorReferenceGuid };
            }
        }

        private PaymentRequestMessage createPaymentRequestMessage(Payment payment,Merchant merchant)
        {
            return new PaymentRequestMessage()
            {
                Amount = payment.Amount, 
                CVV = payment.CVV,
                CardHolderName = payment.CardHolderName,
                CardNumber = payment.CardNumber,
                Currency = payment.Currency,
                MerchantAccountNumber = merchant.AccountNumber,
                MerchantSortCode = merchant.SortCode,
                MonthExpiryDate = payment.MonthExpiryDate,
                YearExpiryDate = payment.YearExpiryDate,
                RequestId = payment.RequestId
            };
        }
    }
}
