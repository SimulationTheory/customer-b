using Newtonsoft.Json;
using PSE.Customer.V1.Models;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Models
{
    public class PremisesSet : IMcfResult
    {
        [JsonProperty("__metadata")]
        public McfMetadata Metadata { get; set; }

        [JsonProperty("Addressinfo")]
        public McfAddress Address { get; set; }

        [JsonProperty("PremiseID")]
        public string PremiseId { get; set; }

        [JsonProperty("PremiseTypeID")]
        public string PremiseTypeId { get; set; }

        public McfList<PremiseInstallation> Installations { get; set; }
    }
}
