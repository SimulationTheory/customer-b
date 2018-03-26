using Newtonsoft.Json;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Returned when phone data is POSTed
    /// </summary>
    public class PostPhoneResponse : IMcfResult
    {
        /// <summary>
        /// Business parner ID
        /// </summary>
        [JsonProperty("AccountID")]
        public long BusinessPartnerId { get; set; }

        /// <summary>
        /// Sequence number
        /// </summary>
        [JsonProperty("SequenceNo")]
        public string SequenceNumber { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        [JsonProperty("PhoneNo")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        [JsonProperty("HomeFlag")]
        public bool IsHome { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        [JsonProperty("StandardFlag")]
        public bool IsStandard { get; set; }

        /// <summary>
        /// Optional extension
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Phone number, not sure how this is different from the other phone number field
        /// </summary>
        [JsonProperty("CompletePhoneNo")]
        public string CompletePhoneNumber { get; set; }

        /// <summary>
        /// Country code, appears to be standard two letter abbreviation (e.g. US)
        /// </summary>
        [JsonProperty("CountryID")]
        public string CountryId { get; set; }

        /// <summary>
        /// Numeric code that should map to home, work, mobile (mapping TBD)
        /// </summary>
        public string PhoneType { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata is ignored
        /// </value>
        public McfMetadata Metadata { get; set; }
    }
}