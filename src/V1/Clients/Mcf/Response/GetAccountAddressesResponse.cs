using Newtonsoft.Json;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Response Object For Get All Addresses For A Bp
    /// </summary>
    public class GetAccountAddressesResponse : IMcfResult
    {
        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>
        /// The account identifier.
        /// </value>
        public long AccountID { get; set; }
        /// <summary>
        /// Gets or sets the address identifier.
        /// </summary>
        /// <value>
        /// The address identifier.
        /// </value>
        public long AddressID { get; set; }
        /// <summary>
        /// Gets or sets the address information.
        /// </summary>
        /// <value>
        /// The address information.
        /// </value>
        public McfAddressinfo AddressInfo { get; set; }
        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public McfMetadata Metadata { get; set; }
    }
}