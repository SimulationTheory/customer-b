using PSE.Customer.V1.Models;
using PSE.Customer.V1.Request;
using System;
using System.Collections.Generic;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using System.Threading.Tasks;

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


        //ReconnectionResponse GetReconnectAmountDueAndCa(string contractAccountId);
        
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
    }
}
