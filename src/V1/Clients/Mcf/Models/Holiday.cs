using Newtonsoft.Json;
using System;

namespace PSE.Customer.V1.Clients.Mcf.Models
{
    /// <summary>
    /// Represents an MCF holiday.
    /// </summary>
    public class Holiday
    {
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        [JsonProperty("Date")]
        public DateTimeOffset Date { get; set; }
    }
}
