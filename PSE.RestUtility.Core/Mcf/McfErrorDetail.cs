using Newtonsoft.Json;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfErrorDetail
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("propertyref")]
        public string PropertyReference { get; set; }
        [JsonProperty("severity")]
        public string Severity { get; set; }
        [JsonProperty("target")]
        public string Target { get; set; }
    }
}