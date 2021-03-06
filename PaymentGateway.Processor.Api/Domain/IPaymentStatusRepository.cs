﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentGateway.Processor.Api.Domain.Entities;

namespace PaymentGateway.Processor.Api.Domain
{
    public interface IPaymentStatusRepository
    {
        Task<PaymentStatus> GetPaymentStatus(Guid paymentId);
        Task AddPaymentStatus(PaymentStatus paymentStatus);

        Task UpdatePaymentStatus(PaymentStatus paymentStatus);
    }
}
