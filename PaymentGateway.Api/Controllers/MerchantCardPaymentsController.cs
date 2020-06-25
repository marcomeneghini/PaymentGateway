using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentGateway.Api.Attributes;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Domain.Exceptions;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Services;
using PaymentGateway.SharedLib.Encryption;

namespace PaymentGateway.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateModel]
    public class MerchantCardPaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        private readonly ILogger<MerchantCardPaymentsController> _logger;

        public MerchantCardPaymentsController(
            IPaymentService paymentService,
            IMapper mapper,
            ILogger<MerchantCardPaymentsController> logger)
        {
            _paymentService = paymentService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ValidationResultModel), 422)] // UnprocessableEntity
        [ProducesResponseType(typeof(ErrorResponseModel), 409)] // conflict
        [ProducesResponseType(typeof(ErrorResponseModel), 404)] // not found
        [ProducesResponseType(typeof(ErrorResponseModel), 500)] // internal server
        [ProducesResponseType(typeof(CreatePaymentResponseModel), 200)] // OK
        public async Task<IActionResult> CreatePayment(CreatePaymentRequestModel request)
        {
            _logger.LogInformation($"CreatePayment:{request}");
             var response = await _paymentService.CreatePayment(_mapper.Map<CreatePaymentRequest>(request));

             _logger.LogInformation($"CreatePayment OK");
            return Ok(_mapper.Map<CreatePaymentResponseModel>(response));
        }
    }
}
