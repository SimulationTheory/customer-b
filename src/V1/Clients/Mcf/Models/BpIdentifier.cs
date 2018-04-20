using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Models
{
    /// <summary>
    /// Business partner identifier PII information
    /// </summary>
    public class BpIdentifier : IMcfResult
    {
        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>
        /// Customer ID or Business Partner ID.
        /// </value>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the identifier institute.
        /// </summary>
        /// <value>
        /// The identifier institute.
        /// </value>
        public string IdInstitute { get; set; }

        /// <summary>
        /// Gets or sets the type of the identifier.
        /// </summary>
        /// <value>
        /// The type of the identifier.
        /// </value>
        public string IdentifierType { get; set; }

        /// <summary>
        /// Gets or sets the identifier entry date.
        /// </summary>
        /// <value>
        /// The identifier entry date.
        /// </value>
        public string IdEntryDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier no.
        /// </summary>
        /// <value>
        /// The identifier no.
        /// </value>
        public string IdentifierNo { get; set; }

        /// <summary>
        /// Gets or sets the identifier valid from date.
        /// </summary>
        /// <value>
        /// The identifier's start of validity
        /// </value>
        public string IdValidFromDate { get; set; }

        /// <summary>
        /// Gets or sets the valid to date.
        /// </summary>
        /// <value>
        /// The identifier's end of validity
        /// </value>
        public string IdValidToDate { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>
        /// The country name.
        /// </value>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the country ISO code.
        /// </summary>
        /// <value>
        /// The country ISO code, usually a two or three letter code.
        /// </value>
        public string CountryIso { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the metadata (Ignored).
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public McfMetadata Metadata { get; set; }
    }
}
