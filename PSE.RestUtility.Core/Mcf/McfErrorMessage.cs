using Newtonsoft.Json;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfErrorMessage
    {
        [JsonProperty("lang")]
        public string Language { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}