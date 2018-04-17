using Newtonsoft.Json;
using System.Collections.Generic;

namespace PSE.Customer.V1.Clients.Mcf.Models
{
    /// <summary>
    /// Represent and intermediate grouping element from MCF.
    /// </summary>
    public class HolidaysNav
    {
        /// <summary>
        /// Gets or sets the list of holidays.
        /// </summary>
        [JsonProperty("results")]
        public List<Holiday> Holidays { get; set; }
    }
}
