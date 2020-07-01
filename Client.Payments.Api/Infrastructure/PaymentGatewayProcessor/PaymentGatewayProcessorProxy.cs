using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Client.Payments.Api.Domain;
using Client.Payments.Api.Domain.Entities;
using Client.Payments.Api.Infrastructure.PaymentGateway;
using Microsoft.Extensions.Configuration;

namespace Client.Payments.Api.Infrastructure.PaymentGatewayProcessor
{
    public class PaymentGatewayProcessorProxy: IPaymentGatewayProcessorProxy
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public PaymentGatewayProcessorProxy(HttpClient httpClient, IMapper mapper)
        {
            _httpClient = httpClient;
            _mapper = mapper;
        }

        public async Task<PaymentStatusResponse> GetPaymentStatusAsync(Guid paymentStatusGuid)
        {
            HttpResponseMessage httpResponse;
          

            httpResponse = await _httpClient.GetAsync(
                $"api/paymentstatuses?paymentId={paymentStatusGuid}");

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                var paymentStatusDto = await httpResponse.Content.ReadAsAsync<PaymentStatusDto>();
                return _mapper.Map<PaymentStatusResponse>(paymentStatusDto);
            }


            if (httpResponse.StatusCode == HttpStatusCode.Conflict ||
                httpResponse.StatusCode == HttpStatusCode.NotFound ||
                httpResponse.StatusCode == HttpStatusCode.BadRequest ||
                httpResponse.StatusCode == HttpStatusCode.InternalServerError)
            {

                var errorResponseDto = await httpResponse.Content.ReadAsAsync<ErrorResponseDto>();

                return new PaymentStatusResponse()
                {
                    ErrorCode = ErrorCodeMapper.MapErrorCodeToDomain(errorResponseDto.ErrorCode),
                    Message = errorResponseDto.Message
                };
            }
            return null;
        }
    }
}
