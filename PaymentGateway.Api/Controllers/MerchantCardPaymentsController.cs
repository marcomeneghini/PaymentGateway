using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Domain.Exceptions;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Services;
using PaymentGateway.SharedLib.Encryption;

namespace PaymentGateway.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantCardPaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public MerchantCardPaymentsController(IPaymentService paymentService,IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> CreatePayment(CreatePaymentRequestModel request)
        {
            //try
            //{
            var response = await _paymentService.CreatePayment(_mapper.Map<CreatePaymentRequest>(request));


            return Ok(_mapper.Map<CreatePaymentResponseModel>(response));
            //}
            //catch (InvalidMerchantException e)
            //{
            //    return BadRequest(new { message = $"Invalid merchant {e.MerchantId} Reason:{e.InvalidMerchantReason.ToString()}" });
            //}
            //catch (RequestAlreadyProcessedException e)
            //{
            //    // this error is thrown to ensure idempotency
            //    // multiple request same request id error
            //    return Conflict(new
            //        { message = $"request {e.RequestId} already present with status {e.Status.ToString()}"});
            //}
            //catch (Exception e)
            //{
            //    return BadRequest(new {message= e.Message } );
            //}

        }
    }
}
