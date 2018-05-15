using Newtonsoft.Json;
using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Clients.Account.Models.Request
{
    /// <summary>
    /// Synchronize Account Request Object
    /// </summary>
    /// <seealso cref="PSE.WebAPI.Core.Interfaces.IAPIRequest" />
    public class SynchronizeAccountRequest : IAPIRequest
    {
        /// <summary>
        /// Gets or sets the business partner identifier.
        /// </summary>
        /// <value>
        /// The business partner identifier.
        /// </value>
        [JsonProperty("business-partner")]
        public long BusinessPartnerId { get; set; }

        /// <summary>
        /// Gets or sets the contract account identifier.
        /// </summary>
        /// <value>
        /// The contract account identifier.
        /// </value>
        [JsonProperty("contract-account")]
        public long ContractAccountId { get; set; }
    }
}
