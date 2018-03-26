using Newtonsoft.Json;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfDeferred
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
