using Newtonsoft.Json;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Json;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Contact information at the business partner level where a location is not available
    /// </summary>
    /// <seealso cref="PSE.RestUtility.Core.Interfaces.IMcfResult" />
    public class BusinessPartnerContactInfoResponse : IMcfResult
    {
        /// <summary>
        /// Gets or sets the business partner identifier.
        /// </summary>
        /// <value>
        /// Business partner ID mapped to account ID in SAP
        /// </value>
        [JsonConverter(typeof(ToStringJsonConverter))]
        [JsonProperty("AccountID")]
        public long BusinessPartnerId { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the full name (often just first and last).
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the account address independent emails.
        /// </summary>
        /// <value>
        /// Collection of the BP level emails
        /// </value>
        public McfResponseResults<GetEmailResponse> AccountAddressIndependentEmails { get; set; }

        /// <summary>
        /// Gets or sets the account address independent phones.
        /// </summary>
        /// <value>
        /// Collection of address independent phones.
        /// </value>
        public McfResponseResults<GetPhoneResponse> AccountAddressIndependentPhones { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata is ignored
        /// </value>
        public McfMetadata Metadata { get; set; }
    }
}
