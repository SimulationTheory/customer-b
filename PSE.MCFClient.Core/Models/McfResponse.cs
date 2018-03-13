using Newtonsoft.Json;

namespace PSE.MCFClient.Core.Models
{
    public class McfResponse<T>
    {
        [JsonProperty("d")]
        public T Result { get; set; }
    }
}