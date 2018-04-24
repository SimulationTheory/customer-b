using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Models;
using PSE.RestUtility.Core.Mcf;
using System.Threading.Tasks;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Service.Enums;

namespace PSE.Customer.V1.Clients.Mcf.Interfaces
{
    /// <summary>
    /// Handles interaction with SAP via MCF calls
    /// </summary>
    public interface IMcfClient
    {
        /// <summary>
        /// Gets the business partner id and associated collection of IdType and Numbers.
        /// </summary>
        /// <param name="request">The search criteria for business partner search.</param>
        /// <param name="requestChannel">The originating request channel.</param>
        /// <returns>A business partner response object.</returns>
        BpSearchResponse GetDuplicateBusinessPartnerIfExists(BpSearchRequest request, RequestChannelEnum requestChannel);

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
        /// Move in late payment
        /// </summary>
        /// <param name="request"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        MoveInResponse PostLateMoveIn(CreateMoveInRequest request, string jwt);

        /// <summary>
        /// POSTs the work or home phone for the business partner
        /// </summary>
        /// <param name="jwt">Java Web Token for authentication</param>
        /// <param name="request">Phone data to save</param>
        /// <returns>Results of POST request</returns>
        /// <remarks>
        /// OData URI:
        /// POST ZERP_UTILITIES_UMC_PSE_SRV/AccountAddressIndependentMobilePhones
        /// </remarks>
        McfResponse<GetPhoneResponse> CreateAddressDependantPhone(string jwt,
            CreateAddressDependantPhoneRequest request);

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
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// OData URI:
        /// PUT /sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/AccountAddresses(AccountID='BP#',AddressID='AD#')
        /// </remarks>
        void UpdateAddress(string jwt, UpdateAddressRequest request);

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
        ///  Gets the standard mailing addresses for business partner
        /// </summary>
        /// <param name="jwt">Java Web Token for authentication</param>
        /// <param name="bpId">Business partner ID</param>
        /// <returns>Large set of information including mobile phone number and email</returns>
        /// <remarks>
        /// OData URI:
        /// ZERP_UTILITIES_UMC_PSE_SRV/Accounts('BP#')/StandardAccountAddress?$format=json
        /// </remarks>
        McfResponse<GetAccountAddressesResponse> GetStandardMailingAddress(string jwt, long bpId);

        /// <summary>
        /// Gets the Mailing Addresses For A Given BP
        /// </summary>
        /// <param name="jwt"></param>
        /// <param name="bpId"></param>
        /// <returns></returns>
        /// <remarks>
        /// OData URI:
        /// GET "/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/Accounts('{bpId}')/AccountAddresses?$format=json"
        /// </remarks>
        McfResponse<McfResponseResults<GetAccountAddressesResponse>> GetMailingAddresses(string jwt, long bpId);

        /// <summary>
        /// Gets the Mailing Addresses For A Given CA
        /// </summary>
        /// <param name="jwt"></param>
        /// <param name="contractAccountId"></param>
        /// <returns></returns>
        /// <remarks>
        /// OData URI:
        /// GET "/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/ContractAccounts(ContractAccountID='{contractAccountId}')?$format=json"
        /// </remarks>
        McfResponse<GetContractAccountResponse> GetContractAccounMailingAddress(string jwt, long contractAccountId);

        /// <summary>
        /// POSTs a new address.
        /// </summary>
        /// <param name="jwt">Java Web Token for authentication</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// OData URI:
        /// POST /sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/AccountAddresses
        /// </remarks>
        McfResponse<CreateAddressResponse> CreateAddress(string jwt, CreateAddressRequest request);

        /// <summary>
        /// Gets business holidays from MCF for a specific date range.
        /// </summary>
        /// <param name="invalidMoveinDatesRequest">The invalid movein dates request.</param>
        /// <returns></returns>
        McfResponse<GetHolidaysResponse> GetInvalidMoveinDates(GetInvalidMoveinDatesRequest invalidMoveinDatesRequest);

        /// <summary>
        /// PUTs address to contract account.
        /// </summary>
        /// <param name="jwt">Java Web Token for authentication</param>
        /// <param name="contractAccountId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// OData URI:
        /// PUT /sap/opu/odata/sap//ZERP_UTILITIES_UMC_PSE_SRV/ContractAccounts('CA#')
        /// </remarks>
        void FixAddressToContractAccount(string jwt, long contractAccountId, FixAddressToContractAccountRequest request);

        MoveInLatePaymentsResponse GetMoveInLatePaymentsResponse(long contractAccountId, string jwt);

        /// <summary>
        /// Creates a Business Partner for Person/Organization or Autherized contact
        /// </summary>
        /// <param name="businesPartnerequest"></param>
        /// <returns></returns>
        Task<CreateBusinessPartnerMcfResponse> CreateBusinessPartner(CreateBusinesspartnerMcfRequest businesPartnerequest);

        /// <summary>
        /// Creates a Customer Interaction Record
        /// </summary>
        /// <param name="request"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        GetCustomerInteractionResponse CreateCustomerInteractionRecord(CreateCustomerInteractionRequest request, string jwt);

        /// <summary>
        /// Gets all personal identifiers for the business partner.
        /// </summary>
        /// <param name="bpId">The business partner identifier.</param>
        /// <returns>Collection of zero or more business partner identifiers</returns>
        McfResponse<McfResponseResults<BpIdentifier>> GetAllIdentifiers(string bpId);

        /// <summary>
        /// Creates an identifier for the business partner
        /// </summary>
        /// <param name="identifier">The identifier with updated values</param>
        /// <returns>Status code or error message</returns>
        McfStatusCodeResponse<BpIdentifier> CreateIdentifier(BpIdentifier identifier);

        /// <summary>
        /// Updates an identifier for the business partner
        /// </summary>
        /// <param name="identifier">The identifier with updated values</param>
        /// <returns>Status code or error message</returns>
        McfStatusCodeResponse UpdateIdentifier(BpIdentifier identifier);

        /// <summary>
        /// Deletes an identifier for the business partner
        /// </summary>
        /// <param name="identifier">The identifier with updated values</param>
        /// <returns>Status code or error message</returns>
        McfStatusCodeResponse DeleteIdentifier(BpIdentifier identifier);
    }
}
