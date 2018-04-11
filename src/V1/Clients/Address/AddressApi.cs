using System.Threading.Tasks;
using Newtonsoft.Json;
using PSE.Customer.V1.Clients.Address.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.WebAPI.Core.Configuration.Interfaces;
using RestSharp;

namespace PSE.Customer.V1.Clients.Address
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="PSE.Customer.V1.Clients.ClientProxy.ClientProxy" />
    /// <seealso cref="PSE.Customer.V1.Clients.Address.Interfaces.IAddressApi" />
    public class AddressApi : ClientProxy.ClientProxy, IAddressApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressApi"/> class.
        /// </summary>
        /// <param name="coreOptions">used to get load config info such as the balancer address</param>
        public AddressApi(ICoreOptions coreOptions) : base(coreOptions) { }

        /// <summary>
        /// To the MCF mailing address asynchronous.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public async Task<IRestResponse<McfAddressinfo>> ToMcfMailingAddressAsync(AddressDefinedType address)
        {
            var request = new RestRequest($"/v{API_VERSION}/address/mailing/mcf", Method.POST);

            var body = JsonConvert.SerializeObject(address);

            request.AddParameter("application/json", body, ParameterType.RequestBody);

            var resp = await ExecuteAsync<McfAddressinfo>(request);

            return resp;
        }
    }
}
