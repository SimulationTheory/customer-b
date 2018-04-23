using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PSE.Customer.V1.Clients.Mcf.Request
{
    /// <summary>
    /// Request for BP search.
    /// </summary>
    public class BpSearchRequest
    {
        /// <summary>
        /// Gets or sets the email address to search for.
        /// </summary>
        [JsonProperty("Email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the first name to search for.
        /// </summary>
        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name to search for.
        /// </summary>
        [JsonProperty("LastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the middle name or middle initial to search for.
        /// </summary>
        [JsonProperty("MiddleName")]
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the organization name to search for.
        /// </summary>
        [JsonProperty("OrgName")]
        public string OrgName { get; set; }

        /// <summary>
        /// Gets or sets the phone number to search for.
        /// </summary>
        [JsonProperty("Phone")]
        public string Phone { get; set; }

        /// <summary>
        ///     Gets or sets the zip code of a previous or current address to search for.
        /// </summary>        
        [JsonProperty("ServiceZip")]
        public string ServiceZip { get; set; }

        /// <summary>
        /// Gets or sets the Tax ID to search for.
        /// </summary>
        [JsonProperty("TaxID")]
        public string TaxID { get; set; }

        /// <summary>
        /// Gets or sets the UBI to search for.
        /// </summary>
        [JsonProperty("UBI")]
        public string UBI { get; set; }
    }
}