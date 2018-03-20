using PSE.WebAPI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Response
{
    public class SignInResponse : IAPIResponse
    {
        public string JwtAccessToken { get; set; }
    }
}
