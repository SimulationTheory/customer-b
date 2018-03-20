using System.Collections.Generic;
using Newtonsoft.Json;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfInnerError
    {
        [JsonProperty("application")]
        public McfApplication Application { get; set; }
        [JsonProperty("transactionid")]
        public string TransactionId { get; set; }
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
        [JsonProperty("Error_Resolution")]
        public McfErrorResolution ErrorResolution { get; set; }
        [JsonProperty("errordetails")]
        public IEnumerable<McfErrorDetail> ErrorDetails { get; set; }
    }
}