using System.Collections.Generic;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Collection of items stored in "Results" property
    /// </summary>
    /// <typeparam name="T">Type of object in source JSON array</typeparam>
    /// <seealso cref="PSE.RestUtility.Core.Interfaces.IMcfResult" />
    public class McfResultsSet<T> : IMcfResult
    {
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>
        /// Zero or more results
        /// </value>
        public IList<T> Results { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata is ignored
        /// </value>
        public McfMetadata Metadata { get; set; }
    }
}
