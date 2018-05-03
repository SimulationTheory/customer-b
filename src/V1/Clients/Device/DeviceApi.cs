using PSE.Customer.V1.Clients.Device.Interfaces;
using PSE.Customer.V1.Clients.Device.Models.Response;
using PSE.Customer.V1.Clients.Extensions;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Configuration.Interfaces;
using PSE.WebAPI.Core.Service.Interfaces;
using RestSharp;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Clients.Device
{
    /// <summary>
    /// Makes calls to the device endpoints 
    /// </summary>
    public class DeviceApi : ClientProxy.ClientProxy, IDeviceApi
    {
        private readonly IRequestContextAdapter _channelContext;


        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceApi"/> class.
        /// </summary>
        /// <param name="coreOptions">The core options.</param>
        /// <param name="channelContext">The channel context.</param>
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

        /// <inheritdoc />
        public async Task<IRestResponse<GetInstallationResponse>> GetInstallationDetail(long installationId)
        {
            var restRequest = new RestRequest($"/v{API_VERSION}/installation/{installationId}/");
            restRequest.SetJwtAuthorization(_channelContext.JWT);
            restRequest.AddHeader("request-channel", _channelContext.RequestChannel.ToString());
            return await ExecuteAsync<GetInstallationResponse>(restRequest);
        }
    }
}
