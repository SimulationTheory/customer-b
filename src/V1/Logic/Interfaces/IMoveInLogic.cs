using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace PSE.Customer.V1.Logic.Interfaces
{
    /// <summary>
    /// Encapsulates logic for MoveInController
    /// </summary>
    public interface IMoveInLogic
    {
        /// <summary>
        /// Posts a cancellation for the given ContractID.
        /// </summary>
        /// <param name="request">A cancel move-in request containing the contract id to cancel.</param>
        /// <returns>A CancelMoveInReponse object.</returns>
        Task<CancelMoveInResponse> PostCancelMoveIn(CancelMoveInRequest request);

        /// <summary>
        /// Get move in late payment costs associated with provided contract account id 
        /// </summary>
        /// <param name="contractAccountId"></param>
        /// /// <param name="reconnectionFlag"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        ReconnectStatusResponse GetMoveInLatePayment(long contractAccountId, bool reconnectionFlag, string jwt);

        /// <summary>
        /// Posts late move in 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="bp"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        Task<IEnumerable<long>> PostPriorMoveIn(MoveInRequest request, long bp, string jwt);

        /// <summary>
        /// Initates a clean move in
        /// </summary>
        /// <param name="request"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        Task<CleanMoveInResponse> PostCleanMoveIn(CleanMoveInRequest request, string jwt);


        /// <summary>
        /// Create a Business Partner
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<CreateBusinesspartnerResponse> CreateBusinessPartner(CreateBusinesspartnerRequest request);

        /// <summary>
        /// Gets the invalid movein dates for a given date range from MCF.
        /// </summary>
        /// <param name="invalidMoveinDatesRequest">The invalid movein dates request.</param>
        /// <returns></returns>
        List<DateTimeOffset> GetInvalidMoveinDates(GetInvalidMoveinDatesRequest invalidMoveinDatesRequest);

        /// <summary>
        /// Gets all ID types but not values for a BP
        /// </summary>
        /// <param name="bpId">Business partner ID</param>
        /// <returns>Array of zero or more ID types for BP</returns>
        Task<List<IdentifierTypeResponse>> GetAllIdTypes(long bpId);

        /// <summary>
        /// Gets all ID types but not values for a BP
        /// </summary>
        /// <param name="bpId">Business partner ID</param>
        /// <param name="type">Represents identifier types such as last 4 SSN, drivers license number, etc.</param>
        /// <returns>Array of zero or one ID types for BP</returns>
        Task<List<IdentifierTypeResponse>> GetIdType(long bpId, IdentifierType type);

        /// <summary>
        /// Creates one ID type for a BP
        /// </summary>
        /// <param name="identifierRequest">Identifier types such as last 4 SSN, drivers license number, etc.</param>
        Task<CreateBpIdTypeResponse> CreateIdType(IdentifierRequest identifierRequest);

        /// <summary>
        /// Updates one ID type for a BP
        /// </summary>
        /// <param name="identifierRequest">Identifier types such as last 4 SSN, drivers license number, etc.</param>
        Task<StatusCodeResponse> UpdateIdType(IdentifierRequest identifierRequest);

        /// <summary>
        /// Validates the identifier by comparing to ID type user already has
        /// </summary>
        /// <param name="identifierRequest">The identifier request.</param>
        /// <returns>true if identifierRequest matches an existing ID</returns>
        Task<bool> ValidateIdType(IdentifierRequest identifierRequest);

        /// <summary>
        /// Checks for an existing Business Partner id.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BpSearchModel> GetDuplicateBusinessPartnerIfExists(BpSearchRequest request);

        /// <summary>
        /// Gets relationships given a business partner id
        /// </summary>
        /// <param name="bpId"></param>
        ///  /// <param name="jwt"></param>
        /// <returns></returns>
        Task<BpRelationshipsResponse> GetBprelationships(string bpId, string jwt);

        /// <summary>
        /// Delete  relationships given a business partner id
        /// </summary>
        /// <param name="bpId"></param>
        ///  /// <param name="jwt"></param>
        /// <returns></returns>
        Task<BpRelationshipUpdateResponse> DeleteBprelationship(string bpId, string jwt, string bpTodelete);

        /// <summary>
        /// /Creates Authorized Contact
        /// </summary>
        /// <param name="authorizedContactRequest"></param>
        /// <param name="bpId"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        Task<AuthorizedContactResponse> CreateAuthorizedContact(AuthorizedContactRequest authorizedContactRequest, string bpId, string jwt);
    }
}