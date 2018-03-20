using PSE.WebAPI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Request
{
    public class SignupRequest : IAPIRequest
    {
        
        public string Username { get; set; }
        
        public string Password { get; set; }

      
        public string BPId { get; set; }

        public string Email { get; set; }
    }
}
