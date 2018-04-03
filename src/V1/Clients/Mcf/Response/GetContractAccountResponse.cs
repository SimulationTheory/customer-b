using Newtonsoft.Json;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Response Object For Get Contract Account Details From MCF
    /// </summary>
    /// <seealso cref="PSE.RestUtility.Core.Interfaces.IMcfResult" />
    public class GetContractAccountResponse : McfBaseAddress, IMcfResult 
    {
        /// <summary>
        /// Gets or sets the address identifier.
        /// </summary>
        /// <value>
        /// The address identifier.
        /// </value>
        [JsonProperty("AccountAddressID")]
        public string AddressID { get; set; }
        /// <summary>
        /// Gets or sets the contract account identifier.
        /// </summary>
        /// <value>
        /// The contract account  identifier.
        /// </value>
        [JsonProperty("ContractAccountID")]
        public long ContractAccountID { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public McfMetadata Metadata { get; set; }
    }
}
