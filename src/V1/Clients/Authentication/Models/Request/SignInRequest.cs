using PSE.WebAPI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Request
{
    public class SignInRequest :IAPIRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }

    }
}
