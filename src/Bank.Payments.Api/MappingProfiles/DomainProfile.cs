using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bank.Payments.Api.Domain;
using Bank.Payments.Api.Models;

namespace Bank.Payments.Api.MappingProfiles
{
    public class DomainProfile:Profile
    {
        public DomainProfile()
        {
            CreateMap<CardPaymentRequest, CardPaymentRequestModel>().ReverseMap();
            CreateMap<CardPaymentResponse, CardPaymentResponseModel>().ReverseMap();
        }
    }
}
