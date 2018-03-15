using PSE.Customer.V1.Clients.Authentication.Interfaces;
using PSE.Customer.V1.Clients.Authentication.Models.Response;
using PSE.Customer.V1.Clients.ClientProxy.Interfaces;
using PSE.WebAPI.Core.Configuration.Interfaces;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Clients.Authentication
{
    public class AuthenticationApi : ClientProxy.ClientProxy, IAuthenticationApi
    {
        public AuthenticationApi(ICoreOptions coreOptions, IApiUser apiUser) : base(coreOptions)
        {
            _apiUser = apiUser ?? throw new ArgumentNullException(nameof(apiUser));
        }

        public Task<IRestResponse<AccountExistsResponse>> GetAccountExists(long bpId)
        {
            var request = new RestRequest($"/v{API_VERSION}/authentication/mypse-account-exists/{bpId}");
            return ExecuteAsync<AccountExistsResponse>(request);
        }
    }
}
