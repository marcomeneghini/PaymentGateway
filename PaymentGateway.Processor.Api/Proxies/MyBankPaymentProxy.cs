using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.Processor.Api.Domain.Entities;
using PaymentGateway.Processor.Api.Domain.Exceptions;

namespace PaymentGateway.Processor.Api.Proxies
{
    public class MyBankPaymentProxy:IBankPaymentProxy
    {
        
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        public MyBankPaymentProxy(
            HttpClient httpClient, 
            IMapper mapper)
        {
            _httpClient = httpClient;
            _mapper = mapper;
        }

        public async Task<PaymentResult> CreatePaymentAsync(CardPayment request)
        {
            CardPaymentResponseDto responseDto;
            HttpResponseMessage response;
            var cardpaymentRequestModel = _mapper.Map<CardPaymentRequestDto>(request);
            try
            {
                response = await _httpClient.PostAsJsonAsync(
                    "api/cardpayments", cardpaymentRequestModel);
                responseDto = await response.Content.ReadAsAsync<CardPaymentResponseDto>();
            }
            catch (Exception e)
            {
                // keep this exception used by Polly to retry teh operation
                throw new BankNotAvailableException(e.Message);
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                // duplicated requestId
                return  new PaymentResult()
                {
                    RequestId = responseDto.RequestId,
                    TransactionStatus = TransactionStatus.Declined.ToString(),
                    ErrorCode = Consts.BANK_REQUESTID_DUPLICATED_ERRORCODE,
                    Message = $"Bank RequestId Conflict. RequestId:{responseDto.RequestId}"
                };
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new PaymentResult()
                {
                    RequestId = responseDto.RequestId,
                    TransactionStatus = TransactionStatus.Declined.ToString(),
                    ErrorCode = Consts.BANK_PAYMENT_WRONGDETAILS_ERRORCODE,
                    Message = $"Wrong payment details. RequestId:{responseDto.RequestId}"
                };
            }

            return _mapper.Map<PaymentResult>(responseDto);
        }
           
    }
}
