using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PaymentGateway.Api.Attributes;
using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Models
{
    public class ValidationResultModel:ErrorResponseModel
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
