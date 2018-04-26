using System.Net;

namespace PSE.Customer.V1.Response
{
    /// <summary>
    /// Response that can be used when no body is returned
    /// </summary>
    public class StatusCodeResponse
    {
        /// <summary>
        /// Error message in case of error coming from the API
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Status code coming from the API
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
