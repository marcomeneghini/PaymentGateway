using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Models;
using PaymentGateway.SharedLib.Encryption;

namespace PaymentGateway.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantEncryptedCardPaymentsController : ControllerBase
    {
        private readonly ICipherService _cipherService;

        public MerchantEncryptedCardPaymentsController(ICipherService cipherService)
        {
            _cipherService = cipherService;
        }
        [HttpPost]
        public IActionResult CreatePayment(CreatePaymentEncryptedRequestModel request)
        {
            return BadRequest();
        }
    }
}
