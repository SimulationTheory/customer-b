using System;
using Newtonsoft.Json;

namespace PSE.Customer.V1.Models
{
    public class PremiseInstallation
    {
        public string MoveInEligibility { get; set; }

        public string InstallationId { get; set; }

        public string DivisionId { get; set; }

        public string PremiseId { get; set; }

        [JsonProperty("MIDateFrom")]
        public DateTimeOffset? MoveInDateFrom { get; set; }

        [JsonProperty("MIDateTo")]
        public DateTimeOffset? MoveInDateTo { get; set; }

        [JsonProperty("FutureMoveIn")]
        public DateTimeOffset? FutureMoveInDate { get; set; }
    }
}
