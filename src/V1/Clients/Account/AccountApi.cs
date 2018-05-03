using Newtonsoft.Json;
using PSE.Customer.V1.Clients.Account.Models.Response;
using PSE.Customer.V1.Clients.Device.Interfaces;
using PSE.Customer.V1.Clients.Extensions;
using PSE.WebAPI.Core.Configuration.Interfaces;
using PSE.WebAPI.Core.Service.Interfaces;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Clients.Account
{
    /// <inheritdoc />
    public class AccountApi : ClientProxy.ClientProxy, IAccountApi
    {
        private readonly IRequestContextAdapter _channelContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountApi"/> class.
        /// </summary>
        /// <param name="coreOptions">The core options.</param>
        /// <param name="channelContext">The channel context.</param>
        public AccountApi(ICoreOptions coreOptions, IRequestContextAdapter channelContext) : base(coreOptions)
        {
            _channelContext = channelContext;
        }

        /// <inheritdoc />
        public async Task<GetContractItemsResponse> GetContractItems(long contractAccountId)
        {
            var restRequest = new RestRequest($"/v{API_VERSION}/account/{contractAccountId}/contractitems");
            restRequest.SetJwtAuthorization(_channelContext.JWT);
            restRequest.AddHeader("request-channel", _channelContext.RequestChannel.ToString());

            var response = await ExecuteAsync<GetContractItemsResponse>(restRequest);
            
            return JsonConvert.DeserializeObject<GetContractItemsResponse>(response.Content);
        }

        /// <inheritdoc />
        public async Task<GetAccountDetailsResponse> GetContractAccountDetails(long contractAccountId)
        {
            var restRequest = new RestRequest($"/v{API_VERSION}/account/{contractAccountId}");
            restRequest.SetJwtAuthorization(_channelContext.JWT);
            restRequest.AddHeader("request-channel", _channelContext.RequestChannel.ToString());

            var response = await ExecuteAsync<GetAccountDetailsResponse>(restRequest);

            return JsonConvert.DeserializeObject<GetAccountDetailsResponse>(response.Content);
        }
    }
}
