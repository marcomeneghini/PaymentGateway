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
            
            //client.DefaultRequestHeaders.Authorization =
            //    new AuthenticationHeaderValue("Bearer", token);
            var cardpaymentRequestModel = _mapper.Map<CardPaymentRequestDto>(request);
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                "CardPayments", cardpaymentRequestModel);
            
            var responseDto= await response.Content.ReadAsAsync<CardPaymentResponseDto>();

            return _mapper.Map<CardPaymentResponse>(responseDto);
        }

        private HttpClient createAndConfigureHttpClient(IHttpClientFactory httpClientFactory)
        {
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(" http://amido-tech-test.herokuapp.com/");

            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}
