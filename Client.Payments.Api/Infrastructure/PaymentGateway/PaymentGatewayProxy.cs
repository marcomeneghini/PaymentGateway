using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using Client.Payments.Api.Domain;
using Client.Payments.Api.Domain.Entities;
using Microsoft.Extensions.Configuration;
using IdentityModel.Client;

namespace Client.Payments.Api.Infrastructure.PaymentGateway
{
    public class PaymentGatewayProxy: IPaymentGatewayProxy
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;
        private readonly ITokenProvider _tokenProvider;
        private readonly IConfiguration _configuration;

        public PaymentGatewayProxy(
            IHttpClientFactory httpClientFactory, 
            IMapper mapper, 
            ITokenProvider tokenProvider,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            _tokenProvider = tokenProvider;
            _configuration = configuration;
        }
        public async Task<PaymentResponse> CreatePaymentAsync(Payment payment)
        {
            

            HttpResponseMessage httpResponse;
            var paymentRequestDto = _mapper.Map<CreatePaymentRequestDto>(payment);

            //--------------------------
            var accessToken = await _tokenProvider.GetAccessToken();
            var client = _httpClientFactory.CreateClient();
            var paymentGatewayAddress = _configuration["PaymentGatewayAddress"];
            client.BaseAddress = new Uri(paymentGatewayAddress);

            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.SetBearerToken(accessToken);
            //--------------------------


            httpResponse = await client.PostAsJsonAsync(
                "api/merchantcardpayments", paymentRequestDto);

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                 var createPaymentResponseDto = await httpResponse.Content.ReadAsAsync<CreatePaymentResponseDto>();
                return _mapper.Map<PaymentResponse>(createPaymentResponseDto);
            }


            if (httpResponse.StatusCode == HttpStatusCode.Conflict ||
                httpResponse.StatusCode == HttpStatusCode.NotFound ||
                httpResponse.StatusCode == HttpStatusCode.BadRequest ||
                httpResponse.StatusCode == HttpStatusCode.InternalServerError )
            {

                var errorResponseDto = await httpResponse.Content.ReadAsAsync<ErrorResponseDto>();
                
                return new PaymentResponse()
                {
                    RequestId = errorResponseDto.RequestId,
                    ErrorCode = ErrorCodeMapper.MapErrorCodeToDomain(errorResponseDto.ErrorCode),
                    Message = errorResponseDto.Message
                };
            }
            return null;
        }
    }
}
