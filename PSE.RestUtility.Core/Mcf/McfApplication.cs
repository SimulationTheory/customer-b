using Newtonsoft.Json;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfApplication
    {
        [JsonProperty("component_id")]
        public string ComponentId { get; set; }
        [JsonProperty("service_namespace")]
        public string ServiceNamespace { get; set; }
        [JsonProperty("service_id")]
        public string ServiceId { get; set; }
        [JsonProperty("service_version")]
        public string ServiceVersion { get; set; }
    }
}