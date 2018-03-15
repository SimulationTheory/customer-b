﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PSE.Customer.Configuration;
using PSE.Customer.V1.Clients.Authentication.Interfaces;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories;
using PSE.Customer.V1.Repositories.Entities;
using PSE.Customer.V1.Repositories.Interfaces;
using PSE.WebAPI.Core.Configuration.Interfaces;
using RestSharp;

namespace PSE.Customer.V1.Logic
{
    /// <summary>
    /// Customer logic class
    /// </summary>
    /// <seealso cref="PSE.Customer.V1.Logic.Interfaces.ICustomerLogic" />
    public class CustomerLogic : ICustomerLogic
    {
        private readonly ILogger<CustomerLogic> _logger;
        private readonly IDistributedCache _redisCache;
        private readonly IMemoryCache _localCache;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ICoreOptions _options;
        private readonly IBPByContractAccountRepository _bpByContractAccountRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAuthenticationApi _authenticationApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerLogic"/> class.
        /// </summary>
        /// <param name="redisCache">The redis cache.</param>
        /// <param name="localCache">The local cache.</param>
        /// <param name="appSettings">The application settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="bpByContractAccountRepository">The bp by contract account repository.</param>
        /// <param name="customerRepository">The customer repository.</param>
        /// <param name="authenticationApi">The authentication API.</param>
        public CustomerLogic(
            IDistributedCache redisCache, 
            IMemoryCache localCache, 
            IOptions<AppSettings> appSettings, 
            ILogger<CustomerLogic> logger, 
            ICoreOptions options,
            IBPByContractAccountRepository bpByContractAccountRepository,
            ICustomerRepository customerRepository,
            IAuthenticationApi authenticationApi)
        {
            _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
            _localCache = localCache ?? throw new ArgumentNullException(nameof(localCache));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
;            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options;
            _bpByContractAccountRepository = bpByContractAccountRepository;
            _customerRepository = customerRepository;
            _authenticationApi = authenticationApi;
        }


        /// <summary>
        /// Gets bp ID & acct status while validating acct ID & fullName.
        /// </summary>
        /// <param name="lookupCustomerRequest">The lookup customer request.</param>
        /// <returns></returns>
        public async Task<LookupCustomerModel> LookupCustomer(LookupCustomerRequest lookupCustomerRequest)
        {
            LookupCustomerModel lookupCustomerModel = new LookupCustomerModel();

            // Validate acct ID with bp ID lookup
            BPByContractAccountEntity bpByContractAccountEntity =
                await _bpByContractAccountRepository.GetBpByContractAccountId(lookupCustomerRequest.ContractAccountNumber);
            if (bpByContractAccountEntity != null)
            {
                // Update return value
                lookupCustomerModel.BPId = bpByContractAccountEntity.BusinessPartner_Id;

                // Get customer for this bp
                CustomerEntity customerEntity = await _customerRepository.GetCustomerByBusinessPartnerId(bpByContractAccountEntity.BusinessPartner_Id);
                if (customerEntity != null)
                {
                    try
                    {
                        // Validate name against Customer table- all uppercase
                        if (lookupCustomerRequest.NameOnBill.ToUpper() != customerEntity.FullName)
                        {
                            // Pass null model to indicate not found
                            lookupCustomerModel = null;
                        }
                        else
                        {
                            // Check for web account in auth
                            var accountExistsResponse = await _authenticationApi.GetAccountExists(customerEntity.BusinessPartnerId);
                            if (accountExistsResponse?.Data?.Exists != null)
                            {
                                // Update return value
                                lookupCustomerModel.HasWebAccount = accountExistsResponse.Data.Exists;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"GetAccountExists API call failed for {nameof(customerEntity.BusinessPartnerId)}: {customerEntity.BusinessPartnerId}");
                        throw;
                    }
                }
                else
                {
                    // Pass null model to indicate not found
                    lookupCustomerModel = null;
                }
            }
            else
            {
                // Pass null model to indicat not found
                lookupCustomerModel = null;
            }

            return lookupCustomerModel;
        }

        /// <summary>
        ///Returns CustomerProfileModel based customer and customer contact information retrieved from Cassandra
        /// </summary>
        /// <param name="contractAccountId"></param>
        /// <returns>Task<CustomerProfileModel></returns>
        public async Task<CustomerProfileModel> GetCustomerProfileAsync(long bpId)
        {
            var getCustomer = _customerRepository.GetCustomerAsync(bpId);

            var getCustomerContact = _customerRepository.GetCustomerContactAsync(bpId);

            await Task.WhenAll(getCustomer, getCustomerContact);

            var customer = getCustomer.Result;

            var customerContact = getCustomerContact.Result;

            var model = customer.ToModel();

            customerContact.AddToModel(model);

            return model;
        }
    }
}