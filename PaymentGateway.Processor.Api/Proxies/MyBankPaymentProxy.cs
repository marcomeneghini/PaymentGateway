using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using PaymentGateway.Processor.Api.Domain;

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
            try
            {
                //client.DefaultRequestHeaders.Authorization =
                //    new AuthenticationHeaderValue("Bearer", token);
                var cardpaymentRequestModel = _mapper.Map<CardPaymentRequestDto>(request);
                var response = await _httpClient.PostAsJsonAsync(
                    "api/cardpayments", cardpaymentRequestModel);
                if (response.IsSuccessStatusCode)
                {
                    var responseDto = await response.Content.ReadAsAsync<CardPaymentResponseDto>();
                    return _mapper.Map<CardPaymentResponse>(responseDto);
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

    }
}
