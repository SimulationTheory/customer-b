using Newtonsoft.Json;
using PSE.Customer.Extensions;
using PSE.Customer.V1.Request;
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
        /// Initializes a new instance of the <see cref="BpIdentifier"/> class.
        /// </summary>
        public BpIdentifier()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BpIdentifier"/> class.
        /// </summary>
        /// <param name="identifierRequest">Source of data to create or update the identifier.</param>
        public BpIdentifier(IdentifierRequest identifierRequest) : this()
        {
            AccountId = identifierRequest.BpId;
            IdentifierType = identifierRequest.IdentifierType.ToString();
            IdentifierNo = identifierRequest.IdentifierNo;
        }

        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>
        /// Customer ID or Business Partner ID.
        /// </value>
        [JsonProperty("AccountID")]
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
        [JsonProperty("Identifiertype")]
        public string IdentifierType { get; set; }

        /// <summary>
        /// Gets or sets the identifier entry date.
        /// </summary>
        /// <value>
        /// The identifier entry date.
        /// </value>
        [JsonProperty("Identrydate")]
        public string IdEntryDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier data.
        /// </summary>
        /// <value>
        /// The identifier data.
        /// </value>
        [JsonProperty("Identifierno")]
        public string IdentifierNo { get; set; }

        /// <summary>
        /// Gets or sets the identifier valid from date.
        /// </summary>
        /// <value>
        /// The identifier's start of validity
        /// </value>
        [JsonProperty("Idvalidfromdate")]
        public string IdValidFromDate { get; set; }

        /// <summary>
        /// Gets or sets the valid to date.
        /// </summary>
        /// <value>
        /// The identifier's end of validity
        /// </value>
        [JsonProperty("Idvalidtodate")]
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

        /// <summary>
        /// Returns JSON string without the PII information
        /// </summary>
        /// <remarks>TODO: Make an attribute like [DoNotLog] or [Sensitive] that prevents field from being logged, but still is in JSON output</remarks>
        /// <param name="formatting">The formatting style (defaults to indented)</param>
        /// <param name="nullValueHandling">How to serialze null values (defaults to ignore)</param>
        /// <returns>JSON formatted string without PII</returns>
        public string ToJsonNoPii(Formatting formatting = Formatting.Indented, NullValueHandling nullValueHandling = NullValueHandling.Ignore)
        {
            return new BpIdentifier
            {
                AccountId = AccountId,
                IdentifierType = IdentifierType,
                IdEntryDate = IdEntryDate,
                IdentifierNo = null,
                IdValidFromDate = IdValidFromDate,
                IdValidToDate = IdValidToDate
            }.ToJson();
        }
    }
}
