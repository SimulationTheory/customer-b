using Newtonsoft.Json;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfError
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("message")]
        public McfErrorMessage Message { get; set; }

    }
}