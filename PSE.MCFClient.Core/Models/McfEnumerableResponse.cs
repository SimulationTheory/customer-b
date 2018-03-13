using System.Collections.Generic;
using Newtonsoft.Json;

namespace PSE.MCFClient.Core.Models
{
    public class McfEnumerableResponse
    {
        public class McfResponse<T>
        {
            [JsonProperty("results")]
            public IEnumerable<T> Results { get; set; }
        }
    }
}