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
        /// Gets or sets a value indicating whether [skip DQM].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [skip DQM]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("skipDqm")]
        public bool SkipDqm { get; set; }
    }
}
