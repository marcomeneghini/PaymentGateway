using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Payments.Api.Domain
{
    public interface ITokenProvider
    {
        Task<string> GetAccessToken();

     
    }
}
