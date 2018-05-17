using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Models
{
    public class EmmaCaseObjectsNav
    {
        public string Element { get; set; }

        public string ObjectType { get; set; }

        [JsonProperty("key")]
        public string AccountId { get; set; }
    }
}
