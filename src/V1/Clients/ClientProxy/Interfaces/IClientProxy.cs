using System.Threading.Tasks;
using RestSharp;

namespace PSE.Customer.V1.Clients.ClientProxy.Interfaces
{
    public interface IClientProxy
    {
        Task<IRestResponse> ExecuteAsync(IRestRequest request);

        Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request) where T : new();
    }
}
