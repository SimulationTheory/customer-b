using System;
using System.Collections.Generic;
using System.Net;
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
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Repositories.Interfaces;
using PSE.WebAPI.Core.Configuration.Interfaces;
using RestSharp;
using Microsoft.AspNetCore.Mvc;

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
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _bpByContractAccountRepository = bpByContractAccountRepository ?? throw new ArgumentNullException(nameof(bpByContractAccountRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository)); 
            _authenticationApi = authenticationApi ?? throw new ArgumentNullException(nameof(authenticationApi)); 
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
                        _logger.LogError("GetAccountExists API call failed for " +
                                         $"{nameof(customerEntity.BusinessPartnerId)}: {customerEntity.BusinessPartnerId}\n" +
                                         $"{e.Message}");
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
                // Pass null model to indicate not found
                lookupCustomerModel = null;
            }

            return lookupCustomerModel;
        }

        /// <summary>
        /// Returns CustomerProfileModel based customer and customer contact information retrieved from Cassandra
        /// </summary>
        /// <param name="bpId">Business partner ID</param>
        /// <returns>Awaitable CustomerProfileModel result</returns>
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

        /// <summary>
        /// Saves the mailing address at the BP level
        /// </summary>
        /// <param name="address">Full mailing address</param>
        /// <param name="bpId">Business partner ID</param>
        /// <returns>Status code of async respository call</returns>
        public async Task PutMailingAddressAsync(AddressDefinedType address, long bpId)
        {
            _logger.LogInformation($"PutMailingAddressAsync({nameof(address)}: {address}," +
                                   $"{nameof(bpId)}: {bpId})");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the email address at the BP level
        /// </summary>
        /// <param name="emailAddress">Customer email address</param>
        /// <param name="bpId">Business partner ID</param>
        /// <returns>Status code of async respository call</returns>
        public async Task PutEmailAddressAsync(string emailAddress, long bpId)
        {
            _logger.LogInformation($"PutEmailAddressAsync({nameof(emailAddress)}: {emailAddress}," +
                                   $"{nameof(bpId)}: {bpId})");

            // This returns an empty set and the IsFullyFetched property is true.
            // There is apparently no way to determine if any rows were updated or not,
            // so unless an exception occurs, NoContent will always be returned.
            var response = await _customerRepository.UpdateCustomerEmailAddress(emailAddress, bpId);
            if (response != null)
            {
                // TODO: Update using MCF too and return status of HTTP PUT
            }
        }

        /// <summary>
        /// Saves the phone numbers at the BP level
        /// </summary>
        /// <param name="phones">Customer's phones</param>
        /// <param name="bpId">Business partner ID</param>
        /// <returns>Status code of async respository call</returns>
        public async Task PutPhoneNumbersAsync(List<Phone> phones, long bpId)
        {
            _logger.LogInformation($"PutEmailAddressAsync({nameof(phones)}: {phones}," +
                                   $"{nameof(bpId)}: {bpId})");
            // TODO: What is the return type?
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates profile
        /// By signing up user in cognito and cassandra calling sign up API
        /// Save security questions by calling the security Questions API
        /// in eash step it will return approrate http status code
        /// </summary>
        /// <param name="webprofile"></param>
        /// <returns></returns>
        public async Task  CreateWebProfileAsync(WebProfile webprofile)
        {
            _logger.LogInformation($"CreateWebProfileAsync({nameof(webprofile)}: {webprofile}");

            //Signs up customer in cognito and cassandra
            var resp = await _authenticationApi.SignUpCustomer(webprofile);
            if (resp.StatusCode != HttpStatusCode.OK)
            {
                var message = resp.Content;
                _logger.LogError($"Unable to  sign up for user name {webprofile?.CustomerCredentials?.UserName} and Bp {webprofile.BPId} with error message from Auth sign up service {message}");
                throw new Exception(message);
            }
            //save security questions
            await SaveSecurityQuestions(webprofile, resp);
        }

        /// <summary>
        /// Checks if the username exists by calling the Authentication service api
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<bool> UserNameExists(string userName)
        {
            bool usernameExists = true;
            // Check for web account in auth
            var userNameExistsResponse = await _authenticationApi.GetUserNameExists(userName);
            if (userNameExistsResponse?.Data?.Exists != null)
            {
                // Update return value
                usernameExists = userNameExistsResponse.Data.Exists;
            }
            return usernameExists;
        }

        #region private methods
        private async Task SaveSecurityQuestions(WebProfile webprofile, IRestResponse<OkResult> resp)
        {
            try
            {
                //gets JwtToken
                var jwttoken = await _authenticationApi.GetJwtToken(webprofile?.CustomerCredentials?.UserName, webprofile?.CustomerCredentials?.Password);

                //Save security questions one at at time    
                if (jwttoken.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(jwttoken?.Data?.JwtAccessToken))
                {
                    var savesecurityQuestions = await _authenticationApi.SaveSecurityQuestions(webprofile, jwttoken.Data.JwtAccessToken);

                    if (savesecurityQuestions.StatusCode != HttpStatusCode.Created)
                    {
                        //throw eception
                        var message = resp.Content;
                        _logger.LogError($"Security Questions can not be created for user name {webprofile?.CustomerCredentials?.UserName} and Bp {webprofile.BPId}");
                        //throw new Exception(message);
                    }
                }
                else
                {
                    var message = jwttoken.Content;
                    _logger.LogError($"Unable to signin and get jwt token after sign up for user name {webprofile?.CustomerCredentials?.UserName} and Bp {webprofile.BPId} with error message from Auth sign in service {message}");
                }
            }
            catch (Exception exp)
            {
                _logger.LogError("Saveing Security questions failed with unexpected error", exp);
            }
        }
        #endregion
    }
}
