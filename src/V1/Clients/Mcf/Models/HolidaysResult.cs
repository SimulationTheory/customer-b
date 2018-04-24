using Newtonsoft.Json;

namespace PSE.Customer.V1.Clients.Mcf.Models
{
    /// <summary>
    /// Represents the base result from the MCF get holiday call.
    /// </summary>
    public class HolidaysResult
    {
        /// <summary>
        /// Gets or sets the holiday calendar used in the MCF call.
        /// </summary>
        [JsonProperty("HolidayCalendar")]
        public string HolidayCalendar { get; set; }

        /// <summary>
        /// Gets or sets the factory calendar used in the MCF call.
        /// </summary>
        [JsonProperty("FactoryCalendar")]
        public string FactoryCalendar { get; set; }

        /// <summary>
        /// Gets or sets the start of the date range check.
        /// </summary>
        [JsonProperty("DateFrom")]
        public string DateFrom { get; set; }

        /// <summary>
        /// Gets or sets the end of the date range check.
        /// </summary>
        [JsonProperty("DateTo")]
        public string DateTo { get; set; }

        /// <summary>
        /// Gets or sets the holidays nav, which point to the list of holidays.
        /// </summary>
        [JsonProperty("HolidaysNav")]
        public HolidaysNav HolidaysNav { get; set; }
    }
}
