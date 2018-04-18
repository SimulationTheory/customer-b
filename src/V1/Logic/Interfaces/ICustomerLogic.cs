using System.Collections.Generic;
using PSE.Customer.V1.Models;
using System.Threading.Tasks;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.RestUtility.Core.Mcf;
using PSE.Customer.V1.Clients.Mcf.Response;
using System;
using PSE.Customer.V1.Request;

namespace PSE.Customer.V1.Logic.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICustomerLogic
    {
        /// <summary>
        /// Gets bp ID and acct status while validating acct ID and fullName.
        /// </summary>
        /// <param name="lookupCustomerRequest">The lookup customer request.</param>
        /// <returns></returns>
        Task<LookupCustomerModel> LookupCustomer(LookupCustomerRequest lookupCustomerRequest);

        /// <summary>
        /// Returns CustomerProfileModel based customer and customer contact information retrieved from Cassandra
        /// </summary>
        /// <param name="bpId">Business partner ID</param>
        /// <returns>Awaitable CustomerProfileModel result</returns>
        Task<CustomerProfileModel> GetCustomerProfileAsync(long bpId);

        /// <summary>
        /// Saves the mailing address at the BP level
        /// </summary>
        /// <param name="address">Full mailing address</param>
        /// <param name="bpId">Business partner ID</param>
        /// <returns>Status code of async respository call</returns>
        Task PutMailingAddressAsync(AddressDefinedType address, long bpId);

        /// <summary>
        /// Saves the email address at the BP level
        /// </summary>
        /// <param name="jwt">Java web token for authentication</param>
        /// <param name="emailAddress">Customer email address</param>
        /// <param name="bpId">Business partner ID</param>
        /// <returns>Status code of async respository call</returns>
        Task PutEmailAddressAsync(string jwt, string emailAddress, long bpId);

        /// <summary>
        /// Saves the cell phone number at the BP level
        /// </summary>
        /// <param name="jwt">Java web token for authentication</param>
        /// <param name="phone">Customer's cell phone</param>
        /// <param name="bpId">Business partner ID</param>
        Task PutPhoneNumberAsync(string jwt, Phone phone, long bpId);

        /// <summary>
        /// Gets JWT token 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<string> GetJWTTokenAsync(string userName, string password);

        /// <summary>
        /// Creates profile
        /// By signing up user in cognito and cassandra calling sign up API
        /// Save security questions by calling the security Questions API
        /// in eash step it will return approrate http status code
        /// </summary>
        /// <param name="webprofile"></param>
        /// <returns></returns>
        Task CreateWebProfileAsync(WebProfile webprofile);

        /// <summary>
        /// Checks if the username exists by calling the Authentication service api
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<bool> UserNameExists(string userName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bpId"></param>
        /// <param name="isStandardOnly"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        Task<IEnumerable<MailingAddressesModel>> GetMailingAddressesAsync(long bpId, bool isStandardOnly, string jwt);

        /// <summary>
        /// Upserts the standard mailing address.
        /// </summary>
        /// <param name="bpId">The bp identifier.</param>
        /// <param name="address">The address.</param>
        /// <param name="jwt">The JWT.</param>
        /// <returns></returns>
        long UpsertStandardMailingAddress(long bpId, AddressDefinedType address, string jwt);

        /// <summary>
        /// Upserts the standard mailing address.
        /// </summary>
        /// <param name="bpId">The bp identifier.</param>
        /// <param name="createCustomerInteraction">The address.</param>
        /// <param name="jwt">The JWT.</param>
        /// <returns></returns>
        Task<GetCustomerInteractionResponse> CreateCustomerInteractionRecord(CreateCustomerInteractionRequest createCustomerInteraction, string jwt);


        
    }  
    
}
