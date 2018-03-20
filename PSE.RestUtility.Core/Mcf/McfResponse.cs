using Newtonsoft.Json;
using PSE.RestUtility.Core.Interfaces;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfResponse<T> where T : IMcfResult
    {
        [JsonProperty("d")]
        public T Result { get; set; }
        [JsonProperty("error")]
        public McfErrorResult Error { get; set; }
    }
}