using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Client.Payments.Api.Domain.Entities;
using Client.Payments.Api.Infrastructure.PaymentGateway;
using Client.Payments.Api.Infrastructure.PaymentGatewayProcessor;
using Client.Payments.Api.Models;

namespace Client.Payments.Api.MappingProfiles
{
    public class DomainProfile:Profile
    {
        public DomainProfile()
        {
            CreateMap<PaymentModel, Payment>().ReverseMap();
            CreateMap<Payment, CreatePaymentRequestDto>().ReverseMap();
            CreateMap<CreatePaymentResponseDto, PaymentResponse>();
            CreateMap<PaymentResponse, PaymentResponseModel>();
            CreateMap<PaymentStatusDto, PaymentStatusResponse>();


        }
    }
}
