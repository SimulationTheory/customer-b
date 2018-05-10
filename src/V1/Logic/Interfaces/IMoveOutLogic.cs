using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Logic.Interfaces
{
    /// <summary>
    /// Contains moveout logic.
    /// </summary>
    public interface IMoveOutLogic
    {
        /// <summary>
        /// Stops service for a given set of installation IDs.
        /// </summary>
        /// <param name="stopServiceRequest">The stop service request.</param>
        /// <returns></returns>
        Task<StopServiceResponse> StopService(MoveOutStopServiceRequest stopServiceRequest);
    }
}
