using Newtonsoft.Json;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfErrorResult
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("message")]
        public McfErrorMessage Message { get; set; }
        [JsonProperty("innererror")]
        public McfInnerError InnerError { get; set; }
    }
}