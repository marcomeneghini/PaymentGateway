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

        public async Task<CardPaymentResponse> CreatePaymentAsync(CardPaymentRequest request)
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
                throw new BankNotAvailableException(e.Message);
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                // duplicated requestId
                throw new RequestIdConflictException(responseDto.RequestId);
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new BankPaymentDetailsException(responseDto.Message);
            }

            return _mapper.Map<CardPaymentResponse>(responseDto);
        }
           
    }
}
