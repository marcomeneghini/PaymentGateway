using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public PaymentStatusesController(IPaymentStatusRepository paymentStatusRepository,IMapper mapper)
        {
            _paymentStatusRepository = paymentStatusRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetByPaymentId(Guid paymentId)
        {
            try
            {
                var paymentStatus = await _paymentStatusRepository.GetPaymentStatus(paymentId);
                if (paymentStatus == null)
                    return NotFound();
                return Ok(_mapper.Map<PaymentStatusModel>(paymentStatus));
            }
            catch (Exception e)
            {
                return BadRequest(new { message=e.Message});
            }
        }
    }
}
