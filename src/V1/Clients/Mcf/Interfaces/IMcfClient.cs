using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Interfaces
{
    /// <summary>
    /// Handles interaction with SAP via MCF calls
    /// </summary>
    public interface IMcfClient
    {
        /// <summary>
        /// GETs the contact information at the business partner level that does not have any location information.
        /// </summary>
        /// <param name="jwt">Java Web Token for authentication</param>
        /// <param name="bpId">Business partner ID</param>
        /// <returns>Large set of information including mobile phone number and email</returns>
        /// <remarks>
        /// OData URI:
        /// GET ZERP_UTILITIES_UMC_PSE_SRV/Accounts('BP#')?$expand=AccountAddressIndependentPhones,AccountAddressIndependentMobilePhones,AccountAddressIndependentEmails
        /// </remarks>
        McfResponse<BusinessPartnerContactInfoResponse> GetBusinessPartnerContactInfo(string jwt, string bpId);

        /// <summary>
        /// POSTs the primary email address
        /// </summary>
        /// <param name="jwt">Java Web Token for authentication</param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// OData URI:
        /// POST ZERP_UTILITIES_UMC_PSE_SRV/AccountAddressIndependentEmails
        /// </remarks>
        McfResponse<GetEmailResponse> CreateBusinessPartnerEmail(string jwt, CreateEmailRequest request);

        /// <summary>
        /// POSTs the mobile phone for the business partner
        /// </summary>
        /// <param name="jwt">Java Web Token for authentication</param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// OData URI:
        /// POST ZERP_UTILITIES_UMC_PSE_SRV/AccountAddressIndependentMobilePhones
        /// </remarks>
        McfResponse<GetPhoneResponse> CreateBusinessPartnerMobilePhone(string jwt,
            CreateAddressIndependantPhoneRequest request);

        /// <summary>
        /// GETs the contact information that is associated with a location.
        /// </summary>
        /// <param name="jwt">Java Web Token for authentication</param>
        /// <param name="bpId">Business partner ID</param>
        /// <returns> Large set of information including work and home phone numbers</returns>
        /// <remarks>
        /// OData URI:
        /// GET ZCRM_UTILITIES_UMC_PSE_SRV/Accounts('BP#')/StandardAccountAddress?$expand=AccountAddressDependentPhones,AccountAddressDependentMobilePhones,AccountAddressDependentEmails 
        /// </remarks>
        McfResponse<ContractAccountContactInfoResponse> GetContractAccountContactInfo(string jwt, string bpId);

        /// <summary>
        /// PUTs the address.
        /// </summary>
        /// <param name="jwt">Java Web Token for authentication</param>
        /// <param name="bpId">Business partner ID</param>
        /// <param name="addressId">The address identifier.</param>
        /// <returns></returns>
        /// <remarks>
        /// OData URI:
        /// PUT ZCRM_UTILITIES_UMC_PSE_SRV/AccountAddresses(AccountID='BP#',AddressID='AD#')
        /// </remarks>
        McfResponse<AddressResponse> UpdateAddress(string jwt, string bpId, string addressId);

        /// <summary>
        /// POSTs the mobile phone for the business partner
        /// </summary>
        /// <param name="jwt">Java Web Token for authentication</param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// OData URI:
        /// /sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/AccountAddressDependentPhones
        /// </remarks>
        McfResponse<GetPhoneResponse> CreateContractAccountPhone(string jwt, CreatePhoneRequest request);

        /// <summary>
        /// Gets the payment arrangement.
        /// </summary>
        /// <param name="jwt">The JWT.</param>
        /// <param name="contractAccountId">The contract account identifier.</param>
        /// <returns></returns>
        McfResponse<PaymentArrangementResponse> GetPaymentArrangement(string jwt, long contractAccountId);

        /// <summary>
        /// Gets the Mailing Addresses
        /// </summary>
        /// <param name="jwt"></param>
        /// <param name="contractAccountId"></param>
        /// <returns></returns>
        McfResponse<McfResponseResults<GetAccountAddressesResponse>> GetMailingAddresses(string jwt, long contractAccountId);
    }
}
