using System.Threading.Tasks;
using PSE.Customer.V1.Clients.Authentication.Models.Response;
using PSE.Customer.V1.Clients.ClientProxy.Interfaces;
using RestSharp;

namespace PSE.Customer.V1.Clients.Authentication.Interfaces
{
    public interface IAuthenticationApi : IClientProxy
    {
        Task<IRestResponse<AccountExistsResponse>> GetAccountExists(long bpId);
    }
}
