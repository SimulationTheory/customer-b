using Newtonsoft.Json;

namespace PSE.Customer.V1.Clients.Mcf.Models
{
    /// <summary>
    /// Object Model For Mailing Addresses From Mcf Get Call
    /// </summary>
    public class McfAddressinfo
    {
        /// <summary>
        /// Gets or sets the care of name.
        /// </summary>
        /// <value>
        /// The name of the co.
        /// </value>
        [JsonProperty("COName")]
        public string COName { get; set; }

        /// <summary>
        /// Gets or sets the standard flag.
        /// </summary>
        /// <value>
        /// The standard flag.
        /// </value>
        [JsonProperty("Standardaddress")]
        public bool StandardAddress { get; set; }

        /// <summary>
        /// Gets or sets the valid from date.
        /// </summary>
        /// <value>
        /// The valid from date.
        /// </value>
        [JsonProperty("Validfromdate")]
        public string ValidFromDate { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>
        /// The city.
        /// </value>
        [JsonProperty("City")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the valid to date.
        /// </summary>
        /// <value>
        /// The valid to date.
        /// </value>
        [JsonProperty("Validtodate")]
        public string ValidToDate { get; set; }

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

        /// <summary>
        /// Gets or sets the type of the address.
        /// </summary>
        /// <value>
        /// The type of the address.
        /// </value>
        //TODO: Maps To The Right Json Property When Exposed By MCF 
        public McfAddressType AddressType { get; set; }
    }
}
