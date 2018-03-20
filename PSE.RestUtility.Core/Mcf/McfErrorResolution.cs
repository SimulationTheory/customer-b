using Newtonsoft.Json;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfErrorResolution
    {
        // ReSharper disable once InconsistentNaming
        [JsonProperty("SAP_Transaction")]
        public string SAP_Transaction { get; set; }
        // ReSharper disable once InconsistentNaming
        [JsonProperty("SAP_Note")]
        public string SAP_Note { get; set; }
    }
}
