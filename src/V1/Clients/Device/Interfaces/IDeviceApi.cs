using System.Threading.Tasks;
using PSE.Customer.V1.Response;
using RestSharp;

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
        /// <param name="installationId"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        Task<IRestResponse<GetInstallationDetailResponse>> GetInstallationDetail(long installationId);
    }
}
