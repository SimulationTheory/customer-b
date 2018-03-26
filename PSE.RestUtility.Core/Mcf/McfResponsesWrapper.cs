using Newtonsoft.Json;
using PSE.RestUtility.Core.Interfaces;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfResponsesWrapper<T> where T : IMcfResult
    {
        [JsonProperty("d")]
        public McfResponses<T> D { get; set; }
    }
}