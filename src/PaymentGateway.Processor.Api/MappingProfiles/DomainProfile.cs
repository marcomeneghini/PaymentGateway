using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PaymentGateway.Processor.Api.Domain.Entities;
using PaymentGateway.Processor.Api.Models;
using PaymentGateway.Processor.Api.Proxies;
using PaymentGateway.SharedLib.Messages;

namespace PaymentGateway.Processor.Api.MappingProfiles
{
    public class DomainProfile:Profile
    {
        public DomainProfile()
        {
            CreateMap<PaymentStatus, PaymentStatusModel>().ReverseMap();

            CreateMap<PaymentResult, CardPaymentResponseDto>().ReverseMap();

            CreateMap<CardPayment, CardPaymentRequestDto>().ReverseMap();

            CreateMap<PaymentRequestMessage, CardPayment>().ReverseMap();
        }
    }
}
