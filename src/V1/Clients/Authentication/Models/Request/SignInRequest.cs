using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Request
{
    public class SignInRequest :IAPIRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }

    }
}
