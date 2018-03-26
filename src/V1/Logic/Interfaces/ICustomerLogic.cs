using System.Collections.Generic;
using PSE.Customer.V1.Models;
using System.Threading.Tasks;
using PSE.Customer.V1.Repositories.DefinedTypes;

namespace PSE.Customer.V1.Logic.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICustomerLogic
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contractAccountId"></param>
        /// <returns></returns>
        Task<CustomerProfileModel> GetCustomerProfileAsync(long contractAccountId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookupCustomerRequest"></param>
        /// <returns></returns>

        Task<LookupCustomerModel> LookupCustomer(LookupCustomerRequest lookupCustomerRequest);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="bpId"></param>
        /// <returns></returns>

        Task PutMailingAddressAsync(AddressDefinedType address, long bpId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="bpId"></param>
        /// <returns></returns>

        Task PutEmailAddressAsync(string emailAddress, long bpId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="phones"></param>
        /// <param name="bpId"></param>
        /// <returns></returns>

        Task PutPhoneNumbersAsync(List<Phone> phones, long bpId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="webprofile"></param>
        /// <returns></returns>

        Task CreateWebProfileAsync(WebProfile webprofile);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userBName"></param>
        /// <returns></returns>

        Task<bool> UserNameExists(string userBName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bpId"></param>
        /// <param name="isStandardOnly"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        //Task<IEnumerable<AddressDefinedType>> GetMailingAddressesAsync(long bpId, bool isStandardOnly,string jwt);
        IEnumerable<AddressDefinedType> GetMailingAddressesAsync(long bpId, bool isStandardOnly, string jwt);
    }
}
