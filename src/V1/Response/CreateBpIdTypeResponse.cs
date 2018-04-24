using System.Net;
using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Response
{
    /// <summary>
    /// A new identifier for the business partner
    /// </summary>
    /// <seealso cref="PSE.WebAPI.Core.Interfaces.IAPIResponse" />
    public class CreateBpIdTypeResponse : IAPIResponse
    {
        /// <summary>
        /// Gets or sets the type of the business partner identifier.
        /// </summary>
        /// <value>
        /// The business partner type identifier type.
        /// </value>
        public IdentifierResponse BpIdType { get; set; }

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
