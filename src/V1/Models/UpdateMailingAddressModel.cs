using Newtonsoft.Json;
using PSE.Customer.V1.Repositories.DefinedTypes;

namespace PSE.Customer.V1.Models
{
    /// <summary>
    /// Update Mailing Address Object model
    /// </summary>
    /// <seealso cref="PSE.Customer.V1.Repositories.DefinedTypes.AddressDefinedType" />
    public class UpdateMailingAddressModel : AddressDefinedType
    {
        /// <summary>
        /// Gets or sets the valid from date.
        /// </summary>
        /// <value>
        /// The valid from date.
        /// </value>
        [JsonProperty("valid-from-date")]
        public string ValidFromDate { get; set; }

        /// <summary>
        /// Gets or sets the valid to date.
        /// </summary>
        /// <value>
        /// The valid to date.
        /// </value>
        [JsonProperty("valid-to-date")]
        public string ValidToDate { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether [skip DQM].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [skip DQM]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("skipDqm")]
        public bool SkipDqm { get; set; }
    }
}
