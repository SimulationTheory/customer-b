using System.Threading.Tasks;
using PSE.Customer.V1.Clients.Device.Interfaces;
using PSE.Customer.V1.Clients.Extensions;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Configuration.Interfaces;
using PSE.WebAPI.Core.Service.Interfaces;
using RestSharp;

namespace PSE.Customer.V1.Clients.Device
{
    /// <summary>
    /// Makes calls to the device endpoints 
    /// </summary>
    public class DeviceApi : ClientProxy.ClientProxy, IDeviceApi
    {
        private readonly IRequestContextAdapter _channelContext;


        public DeviceApi(ICoreOptions coreOptions, IRequestContextAdapter channelContext) : base(coreOptions)
        {
            _channelContext = channelContext;
        }

        /// <inheritdoc />
        public async Task<IRestResponse<GetPremiseInstallationsResponse>> GetPremiseInstallation(long premiseId)
        {
            var restRequest = new RestRequest($"/v{API_VERSION}/installation/premise/{premiseId}/installations");
            restRequest.SetJwtAuthorization(_channelContext.JWT);
            restRequest.AddHeader("request-channel", _channelContext.RequestChannel.ToString());
            return await ExecuteAsync<GetPremiseInstallationsResponse>(restRequest);
        }

        public async Task<IRestResponse<GetInstallationDetailResponse>> GetInstallationDetail(long installationId)
        {
            var restRequest = new RestRequest($"/v{API_VERSION}/installation/installation/{installationId}/");
            restRequest.SetJwtAuthorization(_channelContext.JWT);
            restRequest.AddHeader("request-channel", _channelContext.RequestChannel.ToString());
            return await ExecuteAsync<GetInstallationDetailResponse>(restRequest);
        }
    }
}
