using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Clients.Account.Models.Request
{
    /// <summary>
    /// CreateAccountRequest Model
    /// </summary>
    /// <seealso cref="PSE.WebAPI.Core.Interfaces.IAPIRequest" />
    public class CreateAccountRequest : IAPIRequest
    {
        /// <summary>
        /// Gets or sets the tenant bp identifier.
        /// </summary>
        /// <value>
        /// The tenant bp identifier.
        /// </value>
        public long TenantBPId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }
    }
}
