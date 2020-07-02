using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Client.Payments.Api.Domain;
using Client.Payments.Api.Models;
using Client.Payments.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

namespace Client.Payments.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentStatusesController : ControllerBase
    {
        private readonly IPaymentGatewayProcessorProxy _paymentGatewayProcessorProxy;
        private readonly IMapper _mapper;

        public PaymentStatusesController(
            IPaymentGatewayProcessorProxy paymentGatewayProcessorProxy,
            IMapper mapper)
        {
            _paymentGatewayProcessorProxy = paymentGatewayProcessorProxy;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaymentStatusModel), 400)]  // bad request / validation
        [ProducesResponseType(typeof(PaymentStatusModel), 200)] // OK
        public async Task<IActionResult> GetByPaymentGuidId(Guid paymentRequestId)
        {
            var response = await _paymentGatewayProcessorProxy.GetPaymentStatusAsync(paymentRequestId);
            
            return Ok(_mapper.Map<PaymentStatusModel>(response));
        }
    }
}
