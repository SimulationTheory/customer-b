using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Clients.Authentication.Models.Response
{
    public class AccountExistsResponse : IAPIResponse
    {
        public bool Exists { get; set; }
    }
}
