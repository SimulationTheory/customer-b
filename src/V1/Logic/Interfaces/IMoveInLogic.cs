using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSE.Customer.V1.Repositories.DefinedTypes;

namespace PSE.Customer.V1.Logic.Interfaces
{
    public interface IMoveInLogic
    {
        /// <summary>
        /// Get move in late payment costs associated with provided contract account id 
        /// </summary>
        /// <param name="contractAccountId"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        ReconnectStatusResponse GetMoveInLatePayment(long contractAccountId, string jwt);

        /// <summary>
        /// Posts late move in 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="bp"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        Task<MoveInResponse> PostLateMoveIn(MoveInRequest request, long bp, string jwt);

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
        void UpdateIdType(IdentifierRequest identifierRequest);

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
        BpSearchModel GetDuplicateBusinessPartnerIfExists(BpSearchRequest request);
    }
}
