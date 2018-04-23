using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfList<T>
    {
        [JsonProperty("results")]
        public IEnumerable<T> Results { get; set; }
    }
}