using System.Collections.Generic;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Gets active payment arrangement
    /// </summary>
    /// <seealso cref="PSE.RestUtility.Core.Interfaces.IMcfResult" />
    public class PaymentArrangementResponse : IMcfResult
    {
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>
        /// Zero or more payment arrangements
        /// </value>
        public List<PaymentArrangement> Results { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata is ignored
        /// </value>
        public McfMetadata Metadata { get; set; }
    }
}
