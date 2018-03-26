using Newtonsoft.Json;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfMetadata1
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}