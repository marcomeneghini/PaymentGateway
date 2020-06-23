using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaymentGateway.Api.Domain.Exceptions;
using PaymentGateway.Api.Models;
using Serilog;

namespace PaymentGateway.Api.Middlewares
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
            catch (InvalidMerchantException e)
            {
                var referenceCode = Guid.NewGuid().ToString();
                _logger.LogError($"InvalidMerchantException:{e}");
                await HandleInvalidMerchantExceptionAsync(context, e, referenceCode);
            }
            catch (RequestAlreadyProcessedException e)
            {
                var referenceCode = Guid.NewGuid().ToString();
                _logger.LogError($"RequestAlreadyProcessedException:{e}");
                await HandleRequestAlreadyProcessedExceptioAsync(context, e, referenceCode);
            }
            catch (Exception exception)
            {
                var referenceCode = Guid.NewGuid().ToString();
                _logger.LogCritical($"Unexpected exception:{exception}");
                await HandleExceptionAsync(context, exception, referenceCode);
            }
        }

        private Task HandleInvalidMerchantExceptionAsync(HttpContext context, InvalidMerchantException exception, string referenceCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)exception.HttpStatusCode;

            var obj = new ErrorResponseModel()
            {
                ReferenceCode = referenceCode,
                ErrorType = nameof(InvalidMerchantException),
                Message = exception.Message
            };
            return context.Response.WriteAsync(JsonConvert.SerializeObject(obj));
        }

        private Task HandleRequestAlreadyProcessedExceptioAsync(HttpContext context, RequestAlreadyProcessedException exception, string referenceCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) exception.HttpStatusCode;

            var obj = new ErrorResponseModel()
            {
                ReferenceCode = referenceCode,
                ErrorType = nameof(RequestAlreadyProcessedException),
                Message = exception.Message
            };
            return context.Response.WriteAsync(JsonConvert.SerializeObject(obj));
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception, string referenceCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var obj = new ErrorResponseModel()
            {
                ReferenceCode = referenceCode,
                
                Message = $"Unmanaged Exception. Message:{exception.Message}"
            };
            return context.Response.WriteAsync(JsonConvert.SerializeObject(obj));
        }
    }
}
