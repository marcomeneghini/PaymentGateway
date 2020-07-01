using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

namespace Client.Payments.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentStatusesController : ControllerBase
    {

        public PaymentStatusesController()
        {
                
        }

        [HttpGet]
        public Task<IActionResult> GetByPaymentId(Guid paymentIGuid)
        {
            throw  new NotImplementedException();
        }
    }
}
