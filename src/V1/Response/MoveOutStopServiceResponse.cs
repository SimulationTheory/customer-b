using PSE.Customer.V1.Models;
using PSE.RestUtility.Core.Mcf;
using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Response
{
    /// <summary>
    /// Response to handle a moveout stop service request.
    /// </summary>
    /// <seealso cref="PSE.WebAPI.Core.Interfaces.IAPIResponse" />
    public class MoveOutStopServiceResponse : IAPIResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether WarmHomeFund is true or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [warm home fund]; otherwise, <c>false</c>.
        /// </value>
        public bool WarmHomeFund { get; set; }
    }
}
