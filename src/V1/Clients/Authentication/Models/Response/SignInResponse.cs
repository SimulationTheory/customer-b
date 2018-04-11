using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Response
{
    public class SignInResponse : IAPIResponse
    {
        public string JwtAccessToken { get; set; }
    }
}
