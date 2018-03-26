using Newtonsoft.Json;
using PSE.RestUtility.Core.Interfaces;
using System.Collections.Generic;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfResponses<T> where T : IMcfResult
    {
        [JsonProperty("results")]
        public IEnumerable<T> Results { get; set; }
        [JsonProperty("error")]
        public McfErrorResult Error { get; set; }
    }
    
}