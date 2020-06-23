using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.Processor.Api.Models;

namespace PaymentGateway.Processor.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentStatusesController : ControllerBase
    {
        private readonly IPaymentStatusRepository _paymentStatusRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PaymentStatusesController(IPaymentStatusRepository paymentStatusRepository,IMapper mapper, ILogger loggerr)
        {
            _paymentStatusRepository = paymentStatusRepository;
            _mapper = mapper;
            _logger = loggerr;
        }

        [HttpGet]
        public async Task<IActionResult> GetByPaymentId(Guid paymentId)
        {
           
            var paymentStatus = await _paymentStatusRepository.GetPaymentStatus(paymentId);
            if (paymentStatus==null)
            {
                var message = $"Payment not found. PaymentId:{paymentId}";
                _logger.LogError(message);
                return NotFound(new ErrorResponseModel()
                {
                    ReferenceCode = Guid.NewGuid().ToString(),
                    ErrorType = "PaymentNotFoundException",
                    Message = message
                });
            }
            return Ok(_mapper.Map<PaymentStatusModel>(paymentStatus));

        }
    }
}
