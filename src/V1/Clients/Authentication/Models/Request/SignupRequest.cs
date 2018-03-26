using PSE.WebAPI.Core.Interfaces;

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
