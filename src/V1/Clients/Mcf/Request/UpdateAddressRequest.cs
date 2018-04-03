using Newtonsoft.Json;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.RestUtility.Core.Json;

namespace PSE.Customer.V1.Clients.Mcf.Request
{
    /// <summary>
    ///  Request Object For Update Address
    /// </summary>
    /// <seealso cref="McfBaseAddress" />
    public class UpdateAddressRequest : McfBaseAddress
    {
        /// <summary>
        /// Gets or sets the address identifier.
        /// </summary>
        /// <value>
        /// The address identifier.
        /// </value>
        [JsonConverter(typeof(ToStringJsonConverter))]
        [JsonProperty("AddressID")]
        public long AddressID { get; set; }
    }
}
