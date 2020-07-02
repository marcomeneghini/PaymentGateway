using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using Client.Payments.Api.Domain;
using Client.Payments.Api.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Client.Payments.Api.Infrastructure.PaymentGateway
{
    public class PaymentGatewayProxy: IPaymentGatewayProxy
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public PaymentGatewayProxy(HttpClient httpClient, IMapper mapper)
        {
            _httpClient = httpClient;
            _mapper = mapper;
        }
        public async Task<PaymentResponse> CreatePaymentAsync(Payment payment)
        {
            HttpResponseMessage httpResponse;
            var paymentRequestDto = _mapper.Map<CreatePaymentRequestDto>(payment);
          
            httpResponse = await _httpClient.PostAsJsonAsync(
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
