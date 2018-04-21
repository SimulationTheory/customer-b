using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.WebAPI.Core.Interfaces;
using System;

namespace PSE.Customer.V1.Request
{
    /// <summary>
    /// Used to create, update or validate an account identifier.
    /// </summary>
    /// <remarks>Additional fields in SAP response not used: Idinstitute, Country, Countryiso, Region</remarks>
    /// <seealso cref="PSE.WebAPI.Core.Interfaces.IAPIRequest" />
    public class IdentifierRequest : IAPIRequest
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