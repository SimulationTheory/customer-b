using System.Net;
using PSE.Customer.V1.Models;
using System.Threading.Tasks;
using PSE.Customer.V1.Repositories.DefinedTypes;

namespace PSE.Customer.V1.Logic.Interfaces
{
    public interface ICustomerLogic
    {
        Task<CustomerProfileModel> GetCustomerProfileAsync(long contractAccountId);

        Task<LookupCustomerModel> LookupCustomer(LookupCustomerRequest lookupCustomerRequest);

        Task<HttpStatusCode> PutSaveMailingAddressAsync(AddressDefinedType address, long bpId);
    }
}
