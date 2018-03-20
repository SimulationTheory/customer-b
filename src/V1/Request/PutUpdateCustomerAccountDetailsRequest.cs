using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Request
{
    public class PutUpdateCustomerAccountDetailsRequest : IAPIRequest
    {
        public string EmailAddress { get; set; }
        public string HomeNumber { get; set; }
        public string MobilePhone { get; set; }
        public bool? AcceptsTermsOfService { get; set; }
    }
}