using PSE.WebAPI.Core.Interfaces;


namespace PSE.Customer.V1.Clients.Authentication.Models.Response
{
    public class ExistsResponse : IAPIResponse
    {
        public bool Exists { get; set; }
    }
}

