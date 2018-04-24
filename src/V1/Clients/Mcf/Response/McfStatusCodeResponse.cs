using System.Net;
using Newtonsoft.Json;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Return type where response is status code with no body
    /// </summary>
    public class McfStatusCodeResponse
    {
        /// <summary>
        /// Status code coming from MCF API
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }

        /// <summary>
        /// Error message in case of Error coming from the MCF API
        /// </summary>
        [JsonProperty("error")]
        public McfErrorResult Error { get; set; }
    }

    /// <summary>
    /// Return type where response is status code with a body
    /// </summary>
    public class McfStatusCodeResponse<T> : McfResponse<T> where T : IMcfResult
    {
        /// <summary>
        /// Status code coming from MCF API
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
