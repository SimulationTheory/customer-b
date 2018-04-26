using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Models
{
    public class MoveInResponse : MoveInContractNavItem, IMcfResult
    {
        [JsonProperty("SDReason")]
        public new string SDReason { get; set; }

        public McfList<MoveInContractNavItem> ContractItemNav { get; set; }

        [JsonExtensionData]
        private IDictionary<string, JToken> ExtraContents { get; set; }

    }
}
