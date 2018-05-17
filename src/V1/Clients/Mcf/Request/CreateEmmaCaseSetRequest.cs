using Newtonsoft.Json;
using PSE.Customer.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Request
{
    public class CreateEmmaCaseSetRequest
    {
        [JsonProperty("CaseCategory")]
        public string CaseCategory { get; set; }

        [JsonProperty("CasePriority")]
        public string CasePriority { get; set; }

        [JsonProperty("ObjectType")]
        public string ObjectType { get; set; }

        [JsonProperty("Key")]
        public string Key { get; set; }

        [JsonProperty("EmmaCaseObjects")]
        public IEnumerable<EmmaCaseObjectsNav> EmmmaCaseObjectsNav { get; set; }

        [JsonProperty("Text")]
        public IEnumerable<EmmaCaseTextNav> EmmmaCaseTextNav { get; set; }
    }
}
