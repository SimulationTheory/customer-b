using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Response returned when updating address with PUT
    /// </summary>
    /// <seealso cref="PSE.RestUtility.Core.Interfaces.IMcfResult" />
    public class AddressResponse : IMcfResult
    {
        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata is ignored
        /// </value>
        public McfMetadata Metadata { get; set; }
    }
}
