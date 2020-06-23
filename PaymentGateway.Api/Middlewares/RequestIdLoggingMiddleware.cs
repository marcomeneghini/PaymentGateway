using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace PaymentGateway.Api.Middlewares
{
    public class RequestIdLoggingMiddleware
    {
        private readonly RequestDelegate next;

        public RequestIdLoggingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            using (LogContext.PushProperty("RId", Guid.NewGuid()))
            {
                await next.Invoke(context);
            }
        }
    }
}
