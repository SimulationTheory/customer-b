using System.Collections.Generic;
using Newtonsoft.Json;
using PSE.RestUtility.Core.Interfaces;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfResponseResults<T> : IMcfResult
    {
        public McfMetadata Metadata { get; set; }
        [JsonProperty("results")]
        public IEnumerable<T> Results { get; set; }
    }
}
