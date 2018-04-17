using Newtonsoft.Json;

namespace PSE.Customer.V1.Clients.Authentication.Models.Response
{
    /// <summary>
    /// 
    /// </summary>
    public class PutSyncUserEmailResponse
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}