using Newtonsoft.Json;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Request
{
    /// <summary>
    /// Put Mailing Address Request
    /// </summary>
    /// <seealso cref="PSE.Customer.V1.Repositories.DefinedTypes.AddressDefinedType" />
    /// <seealso cref="PSE.WebAPI.Core.Interfaces.IAPIRequest" />
    public class UpdateMailingAddressRequest : AddressDefinedType, IAPIRequest
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
