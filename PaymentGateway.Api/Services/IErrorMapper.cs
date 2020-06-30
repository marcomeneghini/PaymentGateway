using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Services
{
    public interface IErrorMapper
    {
        string GetMessage(string errCode);
        string GetMessage(string errCode,string parameter);

    }
}
