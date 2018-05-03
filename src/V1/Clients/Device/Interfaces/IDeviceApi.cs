using PSE.Customer.V1.Clients.Device.Models.Response;
using PSE.Customer.V1.Response;
using RestSharp;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Clients.Device.Interfaces
{
    public interface IDeviceApi
    {
        /// <summary>
        /// Retrieves premise and installation details associated with provided premise id
        /// </summary>
        /// <param name="premiseId"></param>
        /// <returns></returns>
        Task<IRestResponse<GetPremiseInstallationsResponse>> GetPremiseInstallation(long premiseId);

        /// <summary>
        /// Retrieves installation details of provided installation ids
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <returns></returns>
        Task<IRestResponse<GetInstallationResponse>> GetInstallationDetail(long installationId);
    }
}
