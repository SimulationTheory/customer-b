using System.Collections.Generic;
using Newtonsoft.Json;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfList<T>
    {
        [JsonProperty("results")]
        public IEnumerable<T> Results { get; set; }
    }
}
