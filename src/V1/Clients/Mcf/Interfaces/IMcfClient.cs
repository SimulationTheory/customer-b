using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Models;
using PSE.RestUtility.Core.Mcf;
using System.Threading.Tasks;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Service.Enums;
using System;
using PSE.Customer.V1.Clients.Account.Models.Response;

namespace PSE.Customer.V1.Clients.Mcf.Interfaces
{
    /// <summary>
    /// Handles interaction with SAP via MCF calls
    /// </summary>
    public interface IMcfClient
    {
        /// <summary>
        /// Gets any matching business partner id and associated collection of IdTypes and Numbers.
        /// </summary>
        /// <param name="request">A business partner search request object: <seealso cref="BpSearchRequest"/></param>
        /// <returns>Returns a business partner search response object: <seealso cref="BpSearchResponse"/></returns>
        Task<BpSearchResponse> GetDuplicateBusinessPartnerIfExists(BpSearchRequest request);

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
        MoveInResponse PostMoveIn(CreateMoveInRequest request, string jwt);

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

        MoveInLatePaymentsResponse GetMoveInLatePaymentsResponse(long contractAccountId, bool reconnectionFlag, string jwt);

        /// <summary>
        /// Creates a Business Partner for Person/Organization or Autherized contact
        /// </summary>
        /// <param name="businesPartnerequest"></param>
        /// <returns>createbprelationship</returns>
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

        /// <summary>
        /// Creates BpRelationship
        /// /// </summary>
        /// <param name="jwt"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        bool CreateBpRelationship(string jwt, BpRelationshipRequest request);

        /// <summary>
        /// Creates a cancellation for move in for a contract id.
        /// </summary>
        /// <param name="request">A cancel move in request object: <seealso cref="CancelMoveInRequest"/></param>
        /// <returns>A cancel move in response object: <seealso cref="PostCancelMoveInMcfResponse"/></returns>
        Task<PostCancelMoveInMcfResponse> PostCancelMoveIn(CancelMoveInRequest request);

        /// <summary>
        /// Gets the BP relationships for a given BP
        /// </summary>
        /// <param name="bpId"></param>
        /// /// <param name="jwt"></param>
        /// <returns></returns>
        Task<BpRelationshipsMcfResponse> GetBprelationships(string bpId, string jwt);

        /// <summary>
        /// Gets Bp Relationships for tenant bp using serv account
        /// </summary>
        /// <param name="tenantBp"></param>
        /// <returns></returns>
        Task<BpRelationshipsMcfResponse> GetBprelationships(string tenantBp);

        /// <summary>
        /// Update the business partner relationshipo
        /// </summary>
        /// <param name="request"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        BpRelationshipUpdateResponse UpdateBusinessPartnerRelationship(BpRelationshipUpdateRequest request, string jwt);


        /// <summary>
        /// Stops the service.
        /// </summary>
        /// <param name="contractItemId">The contract item identifier.</param>
        /// <param name="premiseId">The premise identifier.</param>
        /// <param name="moveoutDate">The moveout date.</param>
        /// <returns></returns>
        McfResponse<GetContractItemMcfResponse> StopService(long contractItemId, long premiseId, DateTimeOffset moveoutDate);

        /// <summary>
        /// Gets the businss partners premises.
        /// </summary>
        /// <param name="bpId">Business partner ID</param>
        /// <returns></returns>
        Task<McfResponse<McfResponseResults<PremisesSet>>> GetPremises(string bpId);

        /// <summary>
        /// Gets the owner accounts.
        /// </summary>
        /// <param name="bpId">Business partner ID</param>
        /// <returns></returns>
        Task<McfResponse<McfResponseResults<OwnerAccountsSet>>> GetOwnerAccounts(string bpId);
        /// <summary>
        /// delete the business partner relationshipo
        /// </summary>
        /// <param name="request"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        Task<BpRelationshipUpdateResponse> DeleteBusinessPartnerRelationship(BpRelationshipUpdateRequest request, string jwt);

    }
}