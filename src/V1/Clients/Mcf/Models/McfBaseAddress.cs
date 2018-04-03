using Newtonsoft.Json;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.RestUtility.Core.Json;

namespace PSE.Customer.V1.Clients.Mcf.Interfaces
{
    /// <summary>
    /// Base Class For MCF Adress Request And Response
    /// </summary>
    public abstract class McfBaseAddress
    {
        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>
        /// The account identifier.
        /// </value>
        [JsonConverter(typeof(ToStringJsonConverter))]
        [JsonProperty("AccountID")]
        public long AccountID { get; set; }
        /// <summary>
        /// Gets or sets the address information.
        /// </summary>
        /// <value>
        /// The address information.
        /// </value>
        public McfAddressinfo AddressInfo { get; set; }
    }
}
