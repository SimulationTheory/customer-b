using System.Threading.Tasks;
using PSE.Customer.V1.Response;

namespace PSE.Customer.V1.Logic.Interfaces
{
    /// <summary>
    /// Logic layer for managing accounts with multiple premises and landlords
    /// </summary>
    public interface IManagePremisesLogic
    {
        /// <summary>
        /// Gets all of the premises
        /// </summary>
        /// <param name="bpId">Business partner ID</param>
        /// <returns></returns>
        Task<GetPremisesResponse> GetPremises(string bpId);

        /// <summary>
        /// Gets all of the owner accounts for landlord
        /// </summary>
        /// <param name="bpId">Business partner ID</param>
        /// <returns></returns>
        Task<GetOwnerAccountsResponse> GetOwnerAccounts(string bpId);
    }
}
