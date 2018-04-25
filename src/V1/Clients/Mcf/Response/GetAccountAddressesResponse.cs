using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Response Object For Get All Addresses For A Bp From MCF
    /// </summary>
    public class GetAccountAddressesResponse : McfBaseAddress, IMcfResult
    {
        /// <summary>
        /// Gets or sets the address identifier.
        /// </summary>
        /// <value>
        /// The address identifier.
        /// </value>
        public long? AddressID { get; set; }
        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public McfMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the address information.
        /// </summary>
        /// <value>
        /// The address information.
        /// </value>
        public new McfAddressinfo AddressInfo { get; set; }
    }
}