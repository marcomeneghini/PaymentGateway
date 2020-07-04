using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentGateway.Api.Attributes;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Domain.Entities;
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
        private readonly IErrorMapper _errorMapper;
        private readonly ILogger<MerchantCardPaymentsController> _logger;

        public MerchantCardPaymentsController(
            IPaymentService paymentService,
            IMapper mapper,
            IErrorMapper errorMapper,
            ILogger<MerchantCardPaymentsController> logger)
        {
            _paymentService = paymentService;
            _mapper = mapper;
            _errorMapper = errorMapper;
            _logger = logger;
        }

        
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ValidationResultModel), 400)]  // bad request / validation
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]     // bad request
        [ProducesResponseType(typeof(ErrorResponseModel), 409)]     // conflict
        [ProducesResponseType(typeof(ErrorResponseModel), 404)]     // not found
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]     // internal server
        [ProducesResponseType(typeof(CreatePaymentResponseModel), 200)] // OK
        public async Task<IActionResult> CreatePayment(CreatePaymentRequestModel request)
        {
            _logger.LogInformation($"CreatePayment:{request}");
             var response = await _paymentService.CreatePayment(_mapper.Map<Payment>(request));
             var referenceCode = Guid.NewGuid();
            if(response.ErrorCode == Consts.DUPLICATE_REQUEST_CODE)
            {
                _logger.LogError($"DUPLICATE_REQUEST. ReferenceCode:{referenceCode}");
                return Conflict(new ErrorResponseModel
                {
                    ErrorCode = Consts.DUPLICATE_REQUEST_CODE,
                    Message = _errorMapper.GetMessage(response.ErrorCode, request.RequestId),
                    RequestId = request.RequestId,
                    ReferenceCode = referenceCode.ToString()
                });
            }
            if (response.ErrorCode == Consts.MERCHANT_INVALID_CODE )
            {
                _logger.LogError($"MERCHANT_INVALID. ReferenceCode:{referenceCode}");
                return BadRequest(new ErrorResponseModel
                {
                    ErrorCode = Consts.MERCHANT_INVALID_CODE,
                    Message = _errorMapper.GetMessage(response.ErrorCode, request.MerchantId.ToString()),
                    RequestId = request.RequestId,
                    ReferenceCode = referenceCode.ToString()
                });
            }
            if ( response.ErrorCode == Consts.MERCHANT_NOT_PRESENT_CODE)
            {
                _logger.LogError($"MERCHANT_NOT_PRESENT. ReferenceCode:{referenceCode}");
                return NotFound(new ErrorResponseModel
                {
                    ErrorCode = Consts.MERCHANT_NOT_PRESENT_CODE,
                    Message = _errorMapper.GetMessage(response.ErrorCode, request.MerchantId.ToString()),
                    RequestId = request.RequestId,
                    ReferenceCode = referenceCode.ToString()
                });
            }

            _logger.LogInformation($"CreatePayment OK");
            return Ok(_mapper.Map<CreatePaymentResponseModel>(response));
        }
    }
}
