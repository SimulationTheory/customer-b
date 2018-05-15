using PSE.WebAPI.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PSE.Customer.V1.Request
{
    /// <summary>
    ///  Authorized contact request
    /// </summary>
    public class AuthorizedContactRequest :IAPIRequest
    {
        public CreateBusinesspartnerRequest AuthorizedContact { get; set; }
        public string TenantBpId { get; set; }
        [Required]
        public string ContractAccountId { get; set; }
    }
}
