using PSE.Customer.V1.Models;
using PSE.WebAPI.Core.Interfaces;
using System.Collections.Generic;

namespace PSE.Customer.V1.Response
{
    /// <summary>
    /// A set of one or more identifiers for a business partner
    /// </summary>
    /// <seealso cref="PSE.WebAPI.Core.Interfaces.IAPIResponse" />
    public class IndentifierResponse : IAPIResponse
    {
        /// <summary>
        /// Gets or sets the identifier types.
        /// </summary>
        /// <value>
        /// The identifier types.
        /// </value>
        public List<IdentifierModel> Identifiers { get; set; }
    }
}
