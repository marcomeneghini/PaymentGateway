using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentGateway.Api.Domain.Entities;
using PaymentGateway.Api.Domain.Exceptions;
using PaymentGateway.Api.Models;
using Serilog;

namespace PaymentGateway.Api.Middleware
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
                await _next(context);
            }
            catch (Exception exception)
            {
                var referenceCode = Guid.NewGuid().ToString();
                _logger.LogCritical($"Unexpected exception:{exception}");
                await HandleExceptionAsync(context, exception, referenceCode);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception, string referenceCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var obj = new ErrorResponseModel()
            {
                ReferenceCode = referenceCode,
                ErrorCode = Consts.UNEXPECTED_ERROR_CODE,
                Message = $"Unmanaged Exception. Message:{exception.Message}"
            };
            return context.Response.WriteAsync(JsonConvert.SerializeObject(obj));
        }
    }
}
