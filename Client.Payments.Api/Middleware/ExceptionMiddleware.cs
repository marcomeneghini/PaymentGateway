using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Client.Payments.Api.Domain.Entities;
using Client.Payments.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Serilog;

namespace Client.Payments.Api.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                _logger.LogInformation($"Start Method:{context.Request.Method} Path:{context.Request.Path}.");
                await _next(context);
                _logger.LogInformation($"End {context.Request.Method}  Path:{context.Request.Path}.");
            }
            catch (Exception exception)
            {
                var referenceCode = Guid.NewGuid().ToString();
                _logger.LogCritical($"Unexpected exception:{exception} - ReferenceCode:{referenceCode}");
                await HandleExceptionAsync(context, exception, referenceCode);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception, string referenceCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var obj = new PaymentResponseModel()
            {
                ReferenceCode = referenceCode,
                ErrorCode = Consts.UKNOWN_ERROR_CODE,
                Message = $"Unmanaged Exception. Message:{exception.Message}"
            };
            return context.Response.WriteAsync(JsonConvert.SerializeObject(obj));
        }
    }
}
