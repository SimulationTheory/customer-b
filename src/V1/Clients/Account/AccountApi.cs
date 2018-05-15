using Newtonsoft.Json;
using PSE.Customer.V1.Clients.Account.Models.Request;
using PSE.Customer.V1.Clients.Account.Models.Response;
using PSE.Customer.V1.Clients.Device.Interfaces;
using PSE.Customer.V1.Clients.Extensions;
using PSE.WebAPI.Core.Configuration.Interfaces;
using PSE.WebAPI.Core.Service.Interfaces;
using RestSharp;
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

        /// <summary>
        /// Gets the contract items for a given contract account ID.
        /// </summary>
        /// <param name="contractAccountId">The contract account identifier.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<GetContractItemsResponse> GetContractItems(long contractAccountId)
        {
            var restRequest = new RestRequest($"/v{API_VERSION}/account/{contractAccountId}/contractitems");
            restRequest.SetJwtAuthorization(_channelContext.JWT);
            restRequest.AddHeader("request-channel", _channelContext.RequestChannel.ToString());

            var response = await ExecuteAsync<GetContractItemsResponse>(restRequest);
            
            return JsonConvert.DeserializeObject<GetContractItemsResponse>(response.Content);
        }

        /// <summary>
        /// Gets the contract account detail for a specific contractAccountId.
        /// </summary>
        /// <param name="contractAccountId">The contract account identifier.</param>
        /// <returns></returns>
        /// <inheritdoc />
        public async Task<GetAccountDetailsResponse> GetContractAccountDetails(long contractAccountId)
        {
            var restRequest = new RestRequest($"/v{API_VERSION}/account/{contractAccountId}");
            restRequest.SetJwtAuthorization(_channelContext.JWT);
            restRequest.AddHeader("request-channel", _channelContext.RequestChannel.ToString());

            var response = await ExecuteAsync<GetAccountDetailsResponse>(restRequest);

            return JsonConvert.DeserializeObject<GetAccountDetailsResponse>(response.Content);
        }

        /// <summary>
        /// Posts the create contract account.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<CreateAccountResponse> PostCreateContractAccount(CreateAccountRequest request)
        {
            var requestBody = JsonConvert.SerializeObject(request);
            var restRequest = new RestRequest($"/v{API_VERSION}/account", Method.POST);
            restRequest.SetJwtAuthorization(_channelContext.JWT);
            restRequest.AddHeader("request-channel", _channelContext.RequestChannel.ToString());
            restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

            var accountInfo = await ExecuteAsync<CreateAccountResponse>(restRequest);

            return accountInfo.Data;
        }

        /// <summary>
        /// Puts the synchronize account asynchronous.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task SynchronizeAccountAsync(SynchronizeAccountRequest user)
        {
            var request = new RestRequest($"/v{API_VERSION}/private/account/{user.ContractAccountId}/sync", Method.PUT);
            var body = JsonConvert.SerializeObject(user);

            request.AddParameter("application/json", body, ParameterType.RequestBody);

            var response = await ExecuteAsync(request);
        }
    }
}
