using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Client.Payments.Api.Domain.Entities;
using Client.Payments.Api.Models;
using Client.Payments.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client.Payments.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;


        public PaymentsController(IPaymentService paymentService,IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(PaymentModel paymentModel)
        {
            var response = await _paymentService.DoPaymentAsync(_mapper.Map<Payment>(paymentModel));

            return Ok(_mapper.Map<PaymentResponseModel>(response));
        }

       
    }
}
