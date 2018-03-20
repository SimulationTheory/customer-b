using System.Collections.Generic;
using PSE.Customer.V1.Models;
using System.Threading.Tasks;
using PSE.Customer.V1.Repositories.DefinedTypes;

namespace PSE.Customer.V1.Logic.Interfaces
{
    public interface ICustomerLogic
    {
        Task<CustomerProfileModel> GetCustomerProfileAsync(long contractAccountId);

        Task<LookupCustomerModel> LookupCustomer(LookupCustomerRequest lookupCustomerRequest);

        Task PutMailingAddressAsync(AddressDefinedType address, long bpId);

        Task PutEmailAddressAsync(string emailAddress, long bpId);

        Task PutPhoneNumbersAsync(List<Phone> phones, long bpId);

        Task CreateWebProfileAsync(WebProfile webprofile);

        Task<bool> UserNameExists(string userBName);
    }
}
