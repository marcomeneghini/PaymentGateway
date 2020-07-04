using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.Processor.Api.Domain.Entities;
using PaymentGateway.Processor.Api.Filters;
using PaymentGateway.Processor.Api.Models;
using PaymentGateway.SharedLib.Validation;

namespace PaymentGateway.Processor.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateModel]
    public class PaymentStatusesController : ControllerBase
    {
        private readonly IPaymentStatusRepository _paymentStatusRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentStatusesController> _logger;

        public PaymentStatusesController(IPaymentStatusRepository paymentStatusRepository,IMapper mapper, ILogger<PaymentStatusesController> logger)
        {
            _paymentStatusRepository = paymentStatusRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ErrorResponseModel), 404)]     // not found
        [ProducesResponseType(typeof(ValidationResultModel), 400)]  // Bad Request
        [ProducesResponseType(typeof(PaymentStatusModel), 200)]     // OK
        public async Task<IActionResult> GetByPaymentId(
            [RegularExpression(RegexValidator.VALID_UUID)] Guid paymentId) {
           
            var paymentStatus = await _paymentStatusRepository.GetPaymentStatus(paymentId);
            if (paymentStatus==null)
            {
                // this reference code links the response returned to the client
                // with the log entry that has more details
                var referenceCode = Guid.NewGuid().ToString();
                var message = $"Payment not found. PaymentId:{paymentId}";
                _logger.LogError(message);
                return NotFound(new ErrorResponseModel()
                {
                    ErrorCode = Consts.PAYMENTSTATUS_NOTFOUND_ERRORCODE,
                    ReferenceCode = referenceCode,
                    Message = message
                });
            }
            return Ok(_mapper.Map<PaymentStatusModel>(paymentStatus));
        }
    }
}
