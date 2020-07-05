using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Client.Payments.Api.Domain;
using Client.Payments.Api.Domain.Entities;
using Client.Payments.Api.Infrastructure.PaymentGateway;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;

namespace Client.Payments.Api.Infrastructure.PaymentGatewayProcessor
{
    public class PaymentGatewayProcessorProxy: IPaymentGatewayProcessorProxy
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;
        private readonly ITokenProvider _tokenProvider;
        private readonly IConfiguration _configuration;

        public PaymentGatewayProcessorProxy(
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

        public async Task<PaymentStatusResponse> GetPaymentStatusAsync(Guid paymentRequestId)
        {
            //--------------------------
            var accessToken = await _tokenProvider.GetAccessToken();
            var client = _httpClientFactory.CreateClient();
            var paymentGatewayProcessorAddress = _configuration["PaymentGatewayProcessorAddress"];
            client.BaseAddress = new Uri(paymentGatewayProcessorAddress);

            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.SetBearerToken(accessToken);
            //--------------------------

            var httpResponse = await client.GetAsync(
                $"api/paymentstatuses?paymentId={paymentRequestId}");

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
