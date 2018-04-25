using Newtonsoft.Json;
using PSE.Customer.V1.Repositories.DefinedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Models
{
    /// <summary>
    /// Augment Address Defined Type
    /// </summary>
    /// <seealso cref="PSE.Customer.V1.Repositories.DefinedTypes.AddressDefinedType" />
    public class Address : AddressDefinedType
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
    }
}
