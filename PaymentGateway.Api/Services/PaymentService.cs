using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Domain.Exceptions;

namespace PaymentGateway.Api.Services
{
    public class PaymentService:IPaymentService
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IMerchantRepository merchantRepository,IPaymentRepository paymentRepository)
        {
            _merchantRepository = merchantRepository;
            _paymentRepository = paymentRepository;
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

                // TODO:Prepare the full payment data to send to the Event Broker


                // set the payment status
                paymentStatus.Status = PaymentStatusEnum.Completed;
                await  _paymentRepository.UpdatePaymentStatus(paymentStatus);
                //TODO: set the correct response
                return new CreatePaymentResponse(){ };
            }
            catch (Exception e)
            {
                paymentStatus.Status = PaymentStatusEnum.Error;
                await _paymentRepository.UpdatePaymentStatus(paymentStatus);
                throw;
            }
          
        }
    }
}
