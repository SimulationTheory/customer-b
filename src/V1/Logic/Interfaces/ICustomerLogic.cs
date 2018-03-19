using System.Collections.Generic;
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

        Task<HttpStatusCode> PutMailingAddressAsync(AddressDefinedType address, long bpId);

        Task<HttpStatusCode> PutEmailAddressAsync(string emailAddress, long bpId);

        Task<HttpStatusCode> PutPhoneNumbersAsync(List<Phone> phones, long bpId);
    }
}
