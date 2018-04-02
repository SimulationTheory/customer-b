using Newtonsoft.Json;
using PSE.Customer.V1.Clients.Mcf.Enums;
using PSE.RestUtility.Core.Json;

namespace PSE.Customer.V1.Clients.Mcf.Request
{
    /// <summary>
    /// Creates a phone number at the account level (with a location)
    /// </summary>
    public class CreateAddressDependantPhoneRequest
    {
        /// <summary>
        /// Gets or sets the business partner identifier.
        /// </summary>
        /// <value>
        /// The business partner identifier.
        /// </value>
        [JsonConverter(typeof(ToStringJsonConverter))]
        [JsonProperty("AccountID")]
        public long BusinessPartnerId { get; set; }

        /// <summary>
        /// Gets or sets the address identifier.
        /// </summary>
        /// <value>
        /// The address identifier to associate the phone number with (standard address)
        /// </value>
        public string AddressId { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        [JsonProperty("PhoneNo")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is home.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is home; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("HomeFlag")]
        public bool IsHome { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is standard.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is standard; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("StandardFlag")]
        public bool IsStandard { get; set; }

        /// <summary>
        /// Gets or sets a phone line extension (optional)
        /// </summary>
        /// <value>
        ///   <c>true</c> if extension; otherwise, <c>false</c>.
        /// </value>
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets the type of the phone
        /// </summary>
        /// <value>
        /// The type of the phone.
        /// </value>
        public string PhoneType { get; set; }
    }
}
