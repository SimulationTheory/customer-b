using Newtonsoft.Json;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Json;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Response Object returned From Creation Of New Address 
    /// </summary>
    /// <seealso cref="PSE.Customer.V1.Clients.Mcf.Interfaces.McfBaseAddress" />
    /// <seealso cref="PSE.RestUtility.Core.Interfaces.IMcfResult" />
    public class CreateAddressResponse : McfBaseAddress, IMcfResult
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

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata is ignored
        /// </value>
        public McfMetadata Metadata { get; set; }
    }
}
