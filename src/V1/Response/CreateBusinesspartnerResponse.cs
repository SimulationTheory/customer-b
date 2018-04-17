using PSE.WebAPI.Core.Interfaces;
using System.Net;

namespace PSE.Customer.V1.Response
{
    public class CreateBusinesspartnerResponse : IAPIResponse
    {
        /// <summary>
        /// Business partner Id created
        /// </summary>
        public string BpId { get; set; }

        /// <summary>
        /// Error message in case of Error coming from the Address APi
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Status code coming from Address API
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}