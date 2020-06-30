using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PaymentGateway.Api.Domain;
using PaymentGateway.Api.Domain.Entities;
using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.MappingProfiles
{
    public class DomainProfile:Profile
    {
        public DomainProfile()
        {
            CreateMap<CreatePaymentRequestModel, Payment>();
            CreateMap<CreatePaymentResponse, CreatePaymentResponseModel>();
        }
    }
}
