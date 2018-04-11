using Newtonsoft.Json;

namespace PSE.Customer.V1.Clients.Mcf.Models
{
    /// <summary>
    /// Object Model For Mailing Addresses From Mcf Get Call
    /// </summary>
    public class McfAddressinfo
    {
        /// <summary>
        /// Gets or sets the standard flag.
        /// </summary>
        /// <value>
        /// The standard flag.
        /// </value>
        [JsonProperty("StandardFlag")]
        public string StandardFlag { get; set; }
        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>
        /// The city.
        /// </value>
        [JsonProperty("City")]
        public string City { get; set; }
        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        /// <value>
        /// The postal code.
        /// </value>
        [JsonProperty("PostalCode")]
        public string PostalCode { get; set; }
        /// <summary>
        /// Gets or sets the po box postal code.
        /// </summary>
        /// <value>
        /// The po box postal code.
        /// </value>
        [JsonProperty("POBoxPostalCode")]
        public string POBoxPostalCode { get; set; }
        /// <summary>
        /// Gets or sets the po box.
        /// </summary>
        /// <value>
        /// The po box.
        /// </value>
        [JsonProperty("POBox")]
        public string POBox { get; set; }
        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        /// <value>
        /// The street.
        /// </value>
        [JsonProperty("Street")]
        public string Street { get; set; }
        /// <summary>
        /// Gets or sets the house no.
        /// </summary>
        /// <value>
        /// The house no.
        /// </value>
        [JsonProperty("HouseNo")]
        public string HouseNo { get; set; }
        /// <summary>
        /// Gets or sets the country identifier.
        /// </summary>
        /// <value>
        /// The country identifier (eg: US).
        /// </value>
        [JsonProperty("CountryID")]
        public string CountryID { get; set; }
        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        [JsonProperty("Region")]
        public string Region { get; set; }
        /// <summary>
        /// Gets or sets the house no2.
        /// </summary>
        /// <value>
        /// The house no2.
        /// </value>
        [JsonProperty("HouseNo2")]
        public string HouseNo2 { get; set; }
    }
}
