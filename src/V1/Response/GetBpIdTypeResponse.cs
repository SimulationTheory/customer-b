using System.Collections.Generic;
using PSE.WebAPI.Core.Interfaces;
using PSE.Customer.V1.Repositories.DefinedTypes;

namespace PSE.Customer.V1.Response
{
    /// <summary>
    /// A set of one or more identifiers for a business partner
    /// </summary>
    /// <seealso cref="PSE.WebAPI.Core.Interfaces.IAPIResponse" />
    public class GetBpIdTypeResponse : IAPIResponse
    {
        /// <summary>
        /// Gets or sets the identifier types.
        /// </summary>
        /// <value>
        /// The identifier types.
        /// </value>
        public List<IdentifierType> Identifiers { get; set; }
    }
}
