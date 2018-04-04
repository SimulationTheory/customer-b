using Newtonsoft.Json;
using PSE.RestUtility.Core.Json;

namespace PSE.Customer.V1.Clients.Mcf.Request
{
    /// <summary>
    /// Request Object To Fix address to Contract Account
    /// </summary>
    public class FixAddressToContractAccountRequest
    {
        /// <summary>
        /// Gets or sets the account address identifier.
        /// </summary>
        /// <value>
        /// The account address identifier.
        /// </value>
        [JsonConverter(typeof(ToStringJsonConverter))]
        [JsonProperty("AddressID")]
        public long AccountAddressID { get; set; }
    }
}
