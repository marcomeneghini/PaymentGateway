using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PaymentGateway.Api.Attributes;

namespace PaymentGateway.Api.Models
{
    public class ValidationResultModel
    {
        public string Message { get; set; }
        public List<ValidationError> Errors { get; }

        public ValidationResultModel(ModelStateDictionary modelState)
        {
            Message = "Validation Failed";
            Errors = modelState?.Keys
                .SelectMany(key => modelState?[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                .ToList();
        }
    }
}
