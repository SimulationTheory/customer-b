﻿using PSE.Customer.V1.Clients.Account.Models.Request;
using PSE.Customer.V1.Clients.Account.Models.Response;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Clients.Device.Interfaces
{
    /// <summary>
    /// Client for call to the Account service.
    /// </summary>
    public interface IAccountApi
    {
        /// <summary>
        /// Gets the contract items for a given contract account ID.
        /// </summary>
        /// <param name="contractAccountId">The contract account identifier.</param>
        /// <returns></returns>
        Task<GetContractItemsResponse> GetContractItems(long contractAccountId);

        /// <summary>
        /// Gets the contract account detail for a specific contractAccountId.
        /// </summary>
        /// <param name="contractAccountId">The contract account identifier.</param>
        /// <returns></returns>
        Task<GetAccountDetailsResponse> GetContractAccountDetails(long contractAccountId);

        /// <summary>
        /// Posts the create contract account.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        Task<CreateAccountResponse> PostCreateContractAccount(CreateAccountRequest request);

        /// <summary>
        /// Synchronizes the account asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        Task SynchronizeAccountAsync(SynchronizeAccountRequest user);
    }
}
