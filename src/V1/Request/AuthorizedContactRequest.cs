using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Request
{
    /// <summary>
    ///  Authorized contact request
    /// </summary>
    public class AuthorizedContactRequest :IAPIRequest
    {
        public CreateBusinesspartnerRequest AuthorizedContact { get; set; }
        public string TenantBpId { get; set; }
    }
}
