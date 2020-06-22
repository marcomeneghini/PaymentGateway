using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PaymentGateway.Processor.Api.Domain;
using PaymentGateway.Processor.Api.Models;
using PaymentGateway.Processor.Api.Proxies;

namespace PaymentGateway.Processor.Api.MappingProfiles
{
    public class DomainProfile:Profile
    {
        public DomainProfile()
        {
            CreateMap<PaymentStatus, PaymentStatusModel>().ReverseMap();

            CreateMap<CardPaymentResponse, CardPaymentResponseDto>().ReverseMap();

            CreateMap<CardPaymentRequest, CardPaymentRequestDto>().ReverseMap();
        }
    }
}
