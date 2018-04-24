using System;
using PSE.Customer.V1.Repositories.DefinedTypes;

namespace PSE.Customer.V1.Response
{
    /// <summary>
    /// Returned when creating bp identifier
    /// </summary>
    public class IdentifierResponse
    {
        /// <summary>
        /// Gets or sets the business partner identifier.
        /// </summary>
        /// <value>
        /// The account business partner identifier.
        /// </value>
        public string BpId { get; set; }

        /// <summary>
        /// Represents identifier types such as last 4 SSN, drivers license number, etc.
        /// </summary>
        public IdentifierType IdentifierType { get; set; }

        /// <summary>
        /// Gets or sets the identifier entry date.
        /// </summary>
        /// <value>
        /// The date the identifier was entered / created.
        /// </value>
        public DateTime IdEntryDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier value
        /// </summary>
        /// <value>
        /// The identifier value
        /// </value>
        public string IdentifierNo { get; set; }
    }
}
