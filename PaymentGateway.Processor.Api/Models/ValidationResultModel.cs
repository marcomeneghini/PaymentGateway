using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PaymentGateway.Processor.Api.Domain.Entities;
using PaymentGateway.Processor.Api.Filters;


namespace PaymentGateway.Processor.Api.Models
{
    public class ValidationResultModel: ErrorResponseModel
    {
       
        public List<ValidationError> Errors { get; }

        public ValidationResultModel(ModelStateDictionary modelState)
        {
            ErrorCode = Consts.VALIDATION_ERROR_CODE;
            Message = "Validation Failed";
            Errors = modelState?.Keys
                .SelectMany(key => modelState?[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                .ToList();
        }
    }
}
