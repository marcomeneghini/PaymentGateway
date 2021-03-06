﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Client.Payments.Api.Domain.Entities;
using Client.Payments.Api.Models;
using Client.Payments.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Client.Payments.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentsController> _logger;


        public PaymentsController(
            IPaymentService paymentService,
            IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
           
        }

        [HttpPost]
        [ProducesResponseType(typeof(PaymentResponseModel), 400)]  // bad request / validation
        [ProducesResponseType(typeof(PaymentResponseModel), 200)] // OK
        public async Task<IActionResult> CreatePayment(PaymentModel paymentModel)
        {
           
            var response = await _paymentService.DoPaymentAsync(_mapper.Map<Payment>(paymentModel));

            return Ok(_mapper.Map<PaymentResponseModel>(response));
        }

       
    }
}
