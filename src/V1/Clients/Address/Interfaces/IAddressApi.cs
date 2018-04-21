using PSE.Customer.V1.Clients.Address.Models.Request;
using PSE.Customer.V1.Clients.ClientProxy.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Models;
using RestSharp;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Clients.Address.Interfaces
{
    /// <summary>
    /// Interface For Interactions With Address Microservice
    /// </summary>
    /// <seealso cref="PSE.Customer.V1.Clients.ClientProxy.Interfaces.IClientProxy" />
    public interface IAddressApi : IClientProxy
    {
        /// <summary>
        /// To the MCF mailing address asynchronous.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        Task<IRestResponse<McfAddressinfo>> ToMcfMailingAddressAsync(AddressDefinedTypeRequest address);
    }
}
