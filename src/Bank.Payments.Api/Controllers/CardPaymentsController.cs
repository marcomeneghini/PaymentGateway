using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bank.Payments.Api.Attributes;
using Bank.Payments.Api.Domain;
using Bank.Payments.Api.Models;
using Bank.Payments.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CardPaymentResponse = Bank.Payments.Api.Domain.CardPaymentResponse;
using TransactionStatus = Bank.Payments.Api.Domain.TransactionStatus;

namespace Bank.Payments.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateModel]
    public class CardPaymentsController : ControllerBase
    {
        private readonly ICardPaymentService _cardPaymentService;
        private readonly IMapper _mapper;

        public CardPaymentsController(ICardPaymentService cardPaymentService,IMapper mapper)
        {
            _cardPaymentService = cardPaymentService;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult CreatePayment(CardPaymentRequestModel request)
        {
            try
            {
                var response = _cardPaymentService.DoPayment(_mapper.Map<CardPaymentRequest>(request));
                if (response.TransactionStatus == TransactionStatus.Succeeded)
                {
                    return Ok(_mapper.Map<CardPaymentResponseModel>(response));
                }

                return BadRequest(_mapper.Map<CardPaymentResponseModel>(response));
            }
            catch (RequestAlreadyProcessedException e)
            {
                // this error is thrown to ensure idempotency
                // multiple request same request id error
                var conflictResponseModel = new CardPaymentResponseModel
                {
                    Message = $"request {e.RequestId} already present with status {e.Status.ToString()}",
                    RequestId = e.RequestId,
                    TransactionStatus = e.Status.ToString()
                };
                return Conflict(conflictResponseModel);
            }
            catch (Exception e)
            {
                var badResponseModel = new CardPaymentResponseModel
                {
                    Message = e.Message,
                    RequestId = request.RequestId,
                    TransactionStatus = Models.TransactionStatus.Declined.ToString()
                };
                return BadRequest(badResponseModel);
            }
           
        }
    }
}
