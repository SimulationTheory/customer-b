using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSE.RestUtility.Core.Json;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Converters;

namespace PSE.Customer.V1.Models
{
    public class EmmaCaseModel
    {

        public string CaseCategory { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [Required]
        public int CasePriority { get; set; }

        public string Authorization { get; set; }

        public string ObjectType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [Required]
        public long Key { get; set; }


        public DateTime OriginalDate { get; set; }

        public DateTime OriginalTime { get; set; }

        public DateTime LogicalDateTime { get; set; }

        public IEnumerable<EmmaCaseObjectsNav> EmmmaCaseObjectsNav { get; set; }


        public IEnumerable<EmmaCaseTextNav> EmmmaCaseTextNav { get; set; }



    }
}
