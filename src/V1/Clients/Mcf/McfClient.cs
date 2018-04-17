using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PSE.Customer.Configuration;
using PSE.Customer.Extensions;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Request;
using PSE.RestUtility.Core.Extensions;
using PSE.RestUtility.Core.Mcf;
using PSE.WebAPI.Core.Configuration;
using PSE.WebAPI.Core.Configuration.Interfaces;
using PSE.WebAPI.Core.Exceptions.Types;
using RestSharp;
using PSE.Customer.Configuration;
using PSE.WebAPI.Core.Configuration;
using PSE.WebAPI.Core.Exceptions.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PSE.Customer.V1.Clients.Mcf
{
    /// <summary>
    /// Handles interaction with SAP via MCF calls
    /// </summary>
    public class McfClient : IMcfClient
    {
        //For sending dates in Mcf requests
        private const string McfDateFormat = "yyyy-MM-ddThh:mm:ss";
        private readonly ICoreOptions _coreOptions;
        private readonly ILogger<McfClient> _logger;
        private readonly string _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="McfClient"/> class.
        /// </summary>
        /// <param name="coreOptions">The core options.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">
        /// coreOptions or logger
        /// </exception>
        public McfClient(ICoreOptions coreOptions, ILogger<McfClient> logger)
        {
            _coreOptions = coreOptions ?? throw new ArgumentNullException(nameof(coreOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(coreOptions));

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            _environment = string.IsNullOrEmpty(environment) ? "Development" : environment;
        }

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
        public McfResponse<BusinessPartnerContactInfoResponse> GetBusinessPartnerContactInfo(string jwt, string bpId)
        {
            McfResponse<BusinessPartnerContactInfoResponse> response;

            try
            {
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var cookies = restUtility.GetMcfCookies(jwt).Result;

                var restRequest = new RestRequest(
                    $"/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/Accounts('{bpId}')?$expand=AccountAddressIndependentEmails," +
                    "AccountAddressIndependentMobilePhones,AccountAddressIndependentPhones", Method.GET);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("Accept", "application/json");

                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);

                response = JsonConvert.DeserializeObject<McfResponse<BusinessPartnerContactInfoResponse>>(restResponse.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return response;
        }

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
        public McfResponse<GetEmailResponse> CreateBusinessPartnerEmail(string jwt, CreateEmailRequest request)
        {
            McfResponse<GetEmailResponse> response;

            try
            {
                _logger.LogInformation($"CreateBusinessPartnerEmail(jwt, {nameof(request)}: {request.ToJson(Formatting.None)})");
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var cookies = restUtility.GetMcfCookies(jwt).Result;

                const string url = "/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/AccountAddressIndependentEmails";
                _logger.LogInformation($"{url}");
                var restRequest = new RestRequest(url, Method.POST);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("Accept", "application/json");
                restRequest.AddJsonBody(request);

                _logger.LogInformation("Making MCF call");
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);

                response = JsonConvert.DeserializeObject<McfResponse<GetEmailResponse>>(restResponse.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return response;
        }

        /// <summary>
        /// POSTs the mobile phone for the business partner
        /// </summary>
        /// <param name="jwt">Java Web Token for authentication</param>
        /// <param name="request">Phone data to save</param>
        /// <returns>Results of POST request</returns>
        /// <remarks>
        /// OData URI:
        /// POST ZERP_UTILITIES_UMC_PSE_SRV/AccountAddressIndependentMobilePhones
        /// </remarks>
        public McfResponse<GetPhoneResponse> CreateBusinessPartnerMobilePhone(string jwt, CreateAddressIndependantPhoneRequest request)
        {
            McfResponse<GetPhoneResponse> response;

            try
            {
                var requestBody = request.ToJson(Formatting.None);
                _logger.LogInformation($"CreateBusinessPartnerMobilePhone(jwt, {nameof(request)}: {requestBody})");
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var cookies = restUtility.GetMcfCookies(jwt).Result;

                const string url = "sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/AccountAddressIndependentPhones";
                var restRequest = new RestRequest(url, Method.POST);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("ContentType", "application/json");
                restRequest.AddHeader("Accept", "application/json");
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                _logger.LogInformation("Making MCF call");
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);

                response = JsonConvert.DeserializeObject<McfResponse<GetPhoneResponse>>(restResponse.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return response;
        }

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
        public McfResponse<GetPhoneResponse> CreateAddressDependantPhone(string jwt, CreateAddressDependantPhoneRequest request)
        {
            McfResponse<GetPhoneResponse> response;

            try
            {
                var requestBody = request.ToJson(Formatting.None);
                _logger.LogInformation($"CreateAddressDependantPhone(jwt, {nameof(request)}: {requestBody})");
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var cookies = restUtility.GetMcfCookies(jwt).Result;

                const string url = "/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/AccountAddressDependentPhones";
                var restRequest = new RestRequest(url, Method.POST);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("ContentType", "application/json");
                restRequest.AddHeader("Accept", "application/json");
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                _logger.LogInformation("Making MCF call");
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);

                response = JsonConvert.DeserializeObject<McfResponse<GetPhoneResponse>>(restResponse.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return response;
        }

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
        public McfResponse<ContractAccountContactInfoResponse> GetContractAccountContactInfo(string jwt, string bpId)
        {
            throw new NotImplementedException();
        }

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
        public void UpdateAddress(string jwt, UpdateAddressRequest request)
        {

            try
            {
                var requestBody = request.ToJson(Formatting.None);
                _logger.LogInformation($"UpdateAddress(jwt, {nameof(request)}: {requestBody})");

                var bpId = request.AccountID;
                var addressId = request.AddressID;

                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var cookies = restUtility.GetMcfCookies(jwt).Result;

                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/AccountAddresses(AccountID='{bpId}',AddressID='{addressId}')", Method.PUT);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("ContentType", "application/json");
                restRequest.AddHeader("Accept", "application/json");
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                _logger.LogInformation("Making MCF call");

                var client = restUtility.GetRestClient(config.McfEndpoint);
                var restResponse = client.Execute(restRequest);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{e.Message} for {nameof(request)}: {request.ToJson(Formatting.None)}");
                throw e;
            }
        }


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
        public McfResponse<GetPhoneResponse> CreateContractAccountPhone(string jwt, CreatePhoneRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the payment arrangement.
        /// </summary>
        /// <param name="jwt">The JWT.</param>
        /// <param name="contractAccountId">The contract account identifier</param>
        /// <returns></returns>
        public McfResponse<PaymentArrangementResponse> GetPaymentArrangement(string jwt, long contractAccountId)
        {
            McfResponse<PaymentArrangementResponse> response;

            try
            {
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var cookies = restUtility.GetMcfCookies(jwt).Result;

                var restRequest = new RestRequest(
                    $"/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/ContractAccounts('{contractAccountId}')/PaymentArrangement?" +
                    "$expand=PaymentArrangementNav/InstallmentPlansNav", Method.GET);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("Accept", "application/json");

                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);

                response = JsonConvert.DeserializeObject<McfResponse<PaymentArrangementResponse>>(restResponse.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return response;
        }

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
        public McfResponse<GetAccountAddressesResponse> GetStandardMailingAddress(string jwt, long bpId)
        {
            McfResponse<GetAccountAddressesResponse> response;

            try
            {
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var cookies = restUtility.GetMcfCookies(jwt).Result;

                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/Accounts('{bpId}')/StandardAccountAddress?$format=json", Method.GET);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("Accept", "application/json");

                var client = restUtility.GetRestClient(config.McfEndpoint);
                var restResponse = client.Execute(restRequest);

                response = JsonConvert.DeserializeObject<McfResponse<GetAccountAddressesResponse>>(restResponse.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return response;
        }

        /// <summary>
        ///  Gets the Mailing Addresses For A Given BP
        /// </summary>
        /// <param name="jwt"></param>
        /// <param name="bpId"></param>
        /// <returns></returns>
        public McfResponse<McfResponseResults<GetAccountAddressesResponse>> GetMailingAddresses(string jwt, long bpId)
        {
            McfResponse<McfResponseResults<GetAccountAddressesResponse>> response;

            try
            {
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var cookies = restUtility.GetMcfCookies(jwt).Result;

                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/Accounts('{bpId}')/AccountAddresses?$format=json", Method.GET);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("Accept", "application/json");

                var client = restUtility.GetRestClient(config.McfEndpoint);
                var restResponse = client.Execute(restRequest);

                response = JsonConvert.DeserializeObject<McfResponse<McfResponseResults<GetAccountAddressesResponse>>>(restResponse.Content);

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return response;
        }
       
             /// <summary>
        /// Gets the Mailing Addresses For A Given CA
        /// </summary>
        /// <param name="jwt"></param>
        /// <param name="contractAccountId"></param>
        /// <returns></returns>
        public McfResponse<GetContractAccountResponse> GetContractAccounMailingAddress(string jwt, long contractAccountId)
        {

            McfResponse<GetContractAccountResponse> response;

            try
            {
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var cookies = restUtility.GetMcfCookies(jwt).Result;

                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/ContractAccounts(ContractAccountID='{contractAccountId}')?$format=json", Method.GET);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("Accept", "application/json");

                var client = restUtility.GetRestClient(config.McfEndpoint);
                var restResponse = client.Execute(restRequest);

                response = JsonConvert.DeserializeObject<McfResponse<GetContractAccountResponse>>(restResponse.Content);

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return response;
        }

        /// <summary>
        /// POSTs a new address.
        /// </summary>
        /// <param name="jwt">Java Web Token for authentication</param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// OData URI:
        /// POST /sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/AccountAddresses
        /// </remarks>
        public McfResponse<CreateAddressResponse> CreateAddress(string jwt, CreateAddressRequest request)
        {
            McfResponse<CreateAddressResponse> response;

            try
            {
                var requestBody = request.ToJson(Formatting.None);
                _logger.LogInformation($"CreateAddress(jwt, {nameof(request)}: {requestBody})");

                var bpId = request.AccountID;

                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var cookies = restUtility.GetMcfCookies(jwt).Result;

                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/AccountAddresses", Method.POST);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("ContentType", "application/json");
                restRequest.AddHeader("Accept", "application/json");
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                _logger.LogInformation("Making MCF call");

                var client = restUtility.GetRestClient(config.McfEndpoint);
                var restResponse = client.Execute(restRequest);

                response = JsonConvert.DeserializeObject<McfResponse<CreateAddressResponse>>(restResponse.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{e.Message} for {nameof(request)}: {request.ToJson(Formatting.None)}");
                throw e;
            }

            return response;
        }

        /// <inheritdoc/>
        public McfResponse<GetHolidaysResponse> GetInvalidMoveinDates(GetInvalidMoveinDatesRequest invalidMoveinDatesRequest)
        {
            McfResponse<GetHolidaysResponse> mcfResponse = null;

            try
            {
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var client = restUtility.GetRestClient(config.McfEndpoint);
                var request = //new RestRequest("/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/FactoryCalHolidaysSet?$filter=HolidayCalendar eq 'Z1' and FactoryCalendar eq 'Z1'and DateFrom eq datetime'2018-03-01T00:00:00' and DateTo eq datetime'2018-03-19T00:00:00'&$expand=HolidaysNav&$format=json", Method.GET);
                new RestRequest("/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/FactoryCalHolidaysSet" +
                "?$filter=HolidayCalendar eq 'Z1' and FactoryCalendar eq 'Z1'and DateFrom eq " +
                $"datetime'{invalidMoveinDatesRequest.DateFrom.ToString(McfDateFormat)}' and DateTo eq datetime'{invalidMoveinDatesRequest.DateTo.ToString(McfDateFormat)}'" +
                "&$expand=HolidaysNav&$format=json", Method.GET);

                // Add anon bypass auth
                var mcfUserName = string.Empty;
                var mcfUserPassword = string.Empty;
                SetMcfAnonCredentials(ref mcfUserName, ref mcfUserPassword);
                request.AddBasicCredentials(mcfUserName, mcfUserPassword);
                request.AddHeader("Accept", "application/json");

                var restResponse = client.Execute(request);
                mcfResponse = JsonConvert.DeserializeObject<McfResponse<GetHolidaysResponse>>(restResponse.Content);
                if (mcfResponse.Error != null && mcfResponse.Result == null)
                {
                    _logger.LogError(mcfResponse.Error.Message.Value);
                    throw new BadRequestException(mcfResponse.Error.Message.Value);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return mcfResponse;
        }

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
        public void FixAddressToContractAccount(string jwt, long contractAccountId, FixAddressToContractAccountRequest request)
        {
            try
            {
                var requestBody = request.ToJson(Formatting.None);
                _logger.LogInformation($"FixAddressToContractAccount(jwt, {nameof(request)}: {requestBody})");

                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var cookies = restUtility.GetMcfCookies(jwt).Result;

                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/ContractAccounts('{contractAccountId}')", Method.PUT);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("ContentType", "application/json");
                restRequest.AddHeader("Accept", "application/json");
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                _logger.LogInformation("Making MCF call");

                var client = restUtility.GetRestClient(config.McfEndpoint);
                var restResponse = client.Execute(restRequest);
                               
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{e.Message} for {nameof(request)}: {request.ToJson(Formatting.None)}");
                throw e;
            }
        }

        /// <summary>
        /// Get reconnection payment details from mcf
        /// </summary>
        /// <param name="contractAccountId"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public MoveInLatePaymentsResponse GetMoveInLatePaymentsResponse(long contractAccountId, string jwt)
        {
            try
            {
                var config = _coreOptions.Configuration;
                _logger.LogInformation($"GetMoveInLatePaymentsResponse(jwt, {nameof(contractAccountId)}: {contractAccountId}");
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var cookies = restUtility.GetMcfCookies(jwt);

                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/LatePaymentsSet?$filter=AccountNo eq '{contractAccountId}' and Reconnect eq ''", Method.GET);
                restRequest.AddCookies(cookies.Result);
                restRequest.AddHeader("Accept", "application/json");

                var restResponse = client.Execute(restRequest);
                var mcfResponse = JsonConvert.DeserializeObject<McfResponse<McfResponseResults<MoveInLatePaymentsResponse>>>(restResponse.Content);

                if (mcfResponse.Error != null && mcfResponse.Result == null)
                {
                    _logger.LogError(mcfResponse.Error.Message.Value);
                    throw new BadRequestException(mcfResponse.Error.Message.Value);
                } 
                return mcfResponse.Result.Results.FirstOrDefault();

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{e.Message} for {nameof(contractAccountId)}: {contractAccountId.ToJson(Formatting.None)}");
                throw e;
            }
        }

        /// <inheritdoc />
        public async Task<CreateBusinessPartnerMcfResponse> CreateBusinessPartner(CreateBusinesspartnerMcfRequest request)
        {
            CreateBusinessPartnerMcfResponse response;

            try
            {
                _logger.LogInformation($"CreateAddress({nameof(request)}: {request.ToJson()})");

                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
              

                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/BusinessPartnerEasySet", Method.POST);
                // Add basic auth for anon service account
                var mcfUserName = string.Empty;
                var mcfUserPassword = string.Empty;
                SetMcfAnonCredentials(ref mcfUserName, ref mcfUserPassword);
                restRequest.AddBasicCredentials(mcfUserName, mcfUserPassword);
                restRequest.AddMcfRequestHeaders();
                
                restRequest.AddJsonBody<CreateBusinesspartnerMcfRequest>(request);

                _logger.LogInformation("Making MCF call");

                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var cancellationTokenSource = new CancellationTokenSource();
                var restResponse = await client.ExecuteTaskAsync(restRequest, cancellationTokenSource.Token);
                if(!restResponse.IsSuccessful)
                {
                    var mcfResponse = JsonConvert.DeserializeObject<McfResponse<CreateBusinessPartnerMcfResponse>>(restResponse.Content);
                    if (mcfResponse.Error != null && mcfResponse.Result == null)
                    {
                        var errormessage = $"BusinessPartnerEasySet call to create Business partner was not successfull with Error {mcfResponse.Error?.Message?.Value}";
                       _logger.LogError(errormessage);
                        throw new Exception(errormessage);
                    }
                   
                }
                
                var mcfOkResponse = JsonConvert.DeserializeObject<McfResponse<CreateBusinessPartnerMcfResponse>>(restResponse.Content);
                response = mcfOkResponse.Result;// mcfResponse.Result.Results.FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{e.Message} for {nameof(request)}: {request.ToJson(Formatting.None)}");
                throw e;
            }

            return response;
        }



        #region Private methods
        //TODO Merge the GetCustomerMcfCredentials and SetMcfAnonCredentials once we verify we can use the UMC_ANM_SRV user for all anonymous
        private void SetMcfAnonCredentials(ref string userName, ref string password)
        {
            var options = new CoreOptions(ServiceConfiguration.AppName);
            ICoreOptions _options = options;
            string _environment = String.Empty;
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            _environment = string.IsNullOrEmpty(environment) ? "Development" : environment;


            var isLocal = (_environment?.Equals("Development") ?? true) || string.IsNullOrEmpty(_environment);
            var mcfCustomerUserNameParamName = $"/CI/MS/{_environment}/MCF/CustomerUserName";
            var mcfCustomerUserPasswordParamName = $"/CI/MS/{_environment}/MCF/CustomerUserPassword";


            if (!isLocal)
            {
                userName = _options.GetValueFromParameterStore(mcfCustomerUserNameParamName, false);
                // TODO: this eventually should be updated to an encrypted param store
                password = _options.GetValueFromParameterStore(mcfCustomerUserPasswordParamName, false);
            }
            else
            {
                var fileLocation = Path.Combine(Directory.GetCurrentDirectory(), "localParameterStore.json");
                var json = File.ReadAllText(fileLocation);
                IEnumerable<KeyValuePair<string, string>> parameters = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(json);

                userName = parameters.FirstOrDefault(x => x.Key.Equals(mcfCustomerUserNameParamName)).Value;
                password = parameters.FirstOrDefault(x => x.Key.Equals(mcfCustomerUserPasswordParamName)).Value;
            }
        }
       
        #endregion
    }
}
