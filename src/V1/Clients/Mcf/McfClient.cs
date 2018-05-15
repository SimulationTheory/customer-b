using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSE.Customer.Configuration;
using PSE.Customer.Extensions;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using PSE.RestUtility.Core.Extensions;
using PSE.RestUtility.Core.Mcf;
using PSE.WebAPI.Core.Configuration;
using PSE.WebAPI.Core.Configuration.Interfaces;
using PSE.WebAPI.Core.Exceptions.Types;
using PSE.WebAPI.Core.Service.Interfaces;
using RestSharp;
using PSE.Customer.V1.Clients.Mcf.Models;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PSE.Customer.V1.Clients.Mcf
{
    /// <summary>
    /// Handles interaction with SAP via MCF calls
    /// </summary>
    public class McfClient : IMcfClient
    {
        //For sending dates in Mcf requests
        private const string McfDateFormat = "yyyy-MM-ddThh:mm:ss";
        private readonly IRequestContextAdapter _requestContext;
        private readonly ICoreOptions _coreOptions;
        private readonly ILogger<McfClient> _logger;
        private readonly string _environment;
        private readonly string _requestChannel;
        /// <summary>
        /// End of line marker for HTTP lines.
        /// </summary>
        private const string CRLF = "\x0d\x0a";
        private const long ServerReturnedAnInvalidOrUnrecognizedResponse = 0x80072f78L;
        private const string lrdServiceUserName = "Landlord/UserName";
        private const string lrdServiceUserPassword = "Landlord/Password";
        private const string customerAnonServiceUserName = "CustomerUserName";
        private const string customerAnonServiceUserPassword = "CustomerUserPassword";


        /// <summary>
        /// Initializes a new instance of the <see cref="McfClient"/> class.
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="coreOptions">The core options.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">
        /// coreOptions or logger
        /// </exception>
        public McfClient(IRequestContextAdapter requestContext, ICoreOptions coreOptions, ILogger<McfClient> logger)
        {
            _requestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
            _coreOptions = coreOptions ?? throw new ArgumentNullException(nameof(coreOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(coreOptions));
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            _environment = string.IsNullOrEmpty(environment) ? "Development" : environment;
        }

        #region Business Partner and Account level services
        /// <inheritdoc/>
        public async Task<BpSearchResponse> GetDuplicateBusinessPartnerIfExists(BpSearchRequest request)
        {

            BpSearchResponse result;

            try
            {
                // Format and log request input
                var requestBody = request.ToJson(Formatting.None);
                _logger.LogInformation($"GetDuplicateBusinessPartnerIfExists({nameof(request)}: {requestBody})");

                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);

                var restRequest = new RestRequest(
                    $"/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/BPSearchSet?$filter=" +
                                                  $" Channel eq '{_requestChannel}'" +
                                                  $" and FirstName eq '{request.FirstName}'" +
                                                  $" and MiddleName eq '{request.MiddleName}'" +
                                                  $" and LastName eq '{request.LastName}'" +
                                                  $" and Phone eq '{request.Phone}'" +
                                                  $" and Email eq '{request.Email}'" +
                                                  $" and ServiceZip eq '{request.ServiceZip}'" +
                                                  $" and OrgName eq '{request.OrgName}'" +
                                                  $" and TaxID eq '{request.TaxID}'" +
                                                  $" and UBI eq '{request.UBI}'" +
                                                  $"&$expand=BPsearchIDinfoSet",
                        Method.GET);

                // Get and add credentials for Anonymous Service user account 
                var mcfUserName = string.Empty;
                var mcfUserPassword = string.Empty;
                SetMcfAnonCredentials(ref mcfUserName, ref mcfUserPassword, customerAnonServiceUserName, customerAnonServiceUserPassword);
                restRequest.AddBasicCredentials(mcfUserName, mcfUserPassword);

                // Add headers
                restRequest.AddHeader("Accept", "application/json");

                // Make call to mcf
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var cancellationTokenSource = new CancellationTokenSource();
                var restResponse = await client.ExecuteTaskAsync(restRequest, cancellationTokenSource.Token);

                // Deserialize response
                var mcfResponse = JsonConvert.DeserializeObject<McfResponse<McfResponseResults<BpSearchResponse>>>(restResponse.Content);

                // Check for null result + an error.
                if (mcfResponse.Error != null && mcfResponse.Result == null)
                {
                    _logger.LogError(mcfResponse.Error.Message.Value, mcfResponse.Error.InnerError);

                    result = new BpSearchResponse()
                    {
                        ReasonCode = $"{restResponse.StatusCode.GetHashCode()} {restResponse.StatusCode.ToString()}",
                        Reason = mcfResponse.Error.Message.Value
                    };
                }
                else
                {
                    result = mcfResponse.Result.Results.FirstOrDefault();
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{e.Message} for {nameof(request)}: {request.ToJson(Formatting.None)}");
                throw e;
            }
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
                IEnumerable<System.Net.Cookie> cookies;

                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                if (!String.IsNullOrEmpty(jwt))
                {
                    cookies = restUtility.GetMcfCookies(jwt, _requestContext.RequestChannel.ToString()).Result;
                }
                else
                {
                    cookies = restUtility.GetMcfCookiesByBpAsync(Int64.Parse(bpId)).Result.Result;
                }

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
                var cookies = restUtility.GetMcfCookies(jwt, _requestContext.RequestChannel.ToString()).Result;

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
                var cookies = restUtility.GetMcfCookies(jwt, _requestContext.RequestChannel.ToString()).Result;

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
                var cookies = restUtility.GetMcfCookies(jwt, _requestContext.RequestChannel.ToString()).Result;

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
                var cookies = restUtility.GetMcfCookies(jwt, _requestContext.RequestChannel.ToString()).Result;

                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/AccountAddresses(AccountID='{bpId}',AddressID='{addressId}')", Method.PUT);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("ContentType", "application/json");
                restRequest.AddHeader("Accept", "application/json");
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                _logger.LogInformation("Making MCF call");

                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{e.Message} for {nameof(request)}: {request.ToJson(Formatting.None)}");
                throw;
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
                var cookies = restUtility.GetMcfCookies(jwt, _requestContext.RequestChannel.ToString()).Result;

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
                IEnumerable<System.Net.Cookie> cookies;
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                if (!String.IsNullOrEmpty(jwt))
                {
                    cookies = restUtility.GetMcfCookies(jwt, _requestContext.RequestChannel.ToString()).Result;
                }
                else
                {
                    cookies = restUtility.GetMcfCookiesByBpAsync(bpId).Result.Result;
                }

                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/Accounts('{bpId}')/StandardAccountAddress?$format=json", Method.GET);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("Accept", "application/json");

                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
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
                var cookies = restUtility.GetMcfCookies(jwt, _requestContext.RequestChannel.ToString()).Result;

                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/Accounts('{bpId}')/AccountAddresses?$format=json", Method.GET);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("Accept", "application/json");

                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
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
                var cookies = restUtility.GetMcfCookies(jwt, _requestContext.RequestChannel.ToString()).Result;

                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/ContractAccounts(ContractAccountID='{contractAccountId}')?$format=json", Method.GET);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("Accept", "application/json");

                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
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
                var cookies = restUtility.GetMcfCookies(jwt, _requestContext.RequestChannel.ToString()).Result;

                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/AccountAddresses", Method.POST);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("ContentType", "application/json");
                restRequest.AddHeader("Accept", "application/json");
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                _logger.LogInformation("Making MCF call");

                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);

                response = JsonConvert.DeserializeObject<McfResponse<CreateAddressResponse>>(restResponse.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{e.Message} for {nameof(request)}: {request.ToJson(Formatting.None)}");
                throw;
            }

            return response;
        }

        /// <summary>
        /// POSTs a new customer interaction record.
        /// </summary>
        /// <param name="jwt">Java Web Token for authentication</param>
        /// <param name="request">Request object</param>
        /// <returns>Response object</returns>
        /// <remarks>
        /// OData URI:
        /// POST /sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/InteractionRecords
        /// </remarks>
        public GetCustomerInteractionResponse CreateCustomerInteractionRecord(CreateCustomerInteractionRequest request, string jwt)
        {
            GetCustomerInteractionResponse response = new GetCustomerInteractionResponse();

            try
            {
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var requestBody = request.ToJson(Formatting.None);
                // URL for creating interaction record post
                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/InteractionRecords", Method.POST);

                if (jwt != null)
                {
                    var cookies = restUtility.GetMcfCookies(jwt, _requestContext.RequestChannel.ToString()).Result;
                    restRequest.AddCookies(cookies);
                }
                else
                {
                    // Add anon bypass auth
                    var mcfUserName = string.Empty;
                    var mcfUserPassword = string.Empty;
                    SetMcfAnonCredentials(ref mcfUserName, ref mcfUserPassword, customerAnonServiceUserName, customerAnonServiceUserPassword);
                    restRequest.AddBasicCredentials(mcfUserName, mcfUserPassword);
                }

                // adding headers
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("ContentType", "application/json");
                restRequest.AddHeader("Accept", "application/json");
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                // executing request and response
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);
                response.Success = restResponse.IsSuccessful.ToString();
                if (response.Success == "True")
                {
                    var responseValues = JObject.Parse(restResponse.Content);
                    response.InteractionRecordID = (string)responseValues["d"]["InteractionRecordID"];
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return response;
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
                var cookies = restUtility.GetMcfCookies(jwt, _requestContext.RequestChannel.ToString()).Result;

                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/ContractAccounts('{contractAccountId}')", Method.PUT);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("ContentType", "application/json");
                restRequest.AddHeader("Accept", "application/json");
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                _logger.LogInformation("Making MCF call");

                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{e.Message} for {nameof(request)}: {request.ToJson(Formatting.None)}");
                throw;
            }
        }

        #endregion

        #region Move in/out

        /// <inheritdoc/>
        public McfResponse<GetHolidaysResponse> GetInvalidMoveinDates(GetInvalidMoveinDatesRequest invalidMoveinDatesRequest)
        {
            McfResponse<GetHolidaysResponse> mcfResponse = null;

            try
            {
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var request = new RestRequest("/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/FactoryCalHolidaysSet" +
                    "?$filter=HolidayCalendar eq 'Z1' and FactoryCalendar eq 'Z1'and DateFrom eq " +
                    $"datetime'{invalidMoveinDatesRequest.DateFrom.ToString(McfDateFormat)}' and DateTo eq datetime'{invalidMoveinDatesRequest.DateTo.ToString(McfDateFormat)}'" +
                    "&$expand=HolidaysNav&$format=json", Method.GET);

                // Add anon bypass auth
                var mcfUserName = string.Empty;
                var mcfUserPassword = string.Empty;
                SetMcfAnonCredentials(ref mcfUserName, ref mcfUserPassword, customerAnonServiceUserName, customerAnonServiceUserPassword);
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
        /// Get reconnection payment details from mcf
        /// </summary>
        /// <param name="contractAccountId"></param>
        /// <param name="jwt"></param>
        ///  /// <param name="reconnectionFlag"></param>
        /// <returns></returns>
        public MoveInLatePaymentsResponse GetMoveInLatePaymentsResponse(long contractAccountId, bool reconnectionFlag, string jwt)
        {
            try
            {
                var config = _coreOptions.Configuration;
                _logger.LogInformation($"GetMoveInLatePaymentsResponse(jwt, {nameof(contractAccountId)}: {contractAccountId}");
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var cookies = restUtility.GetMcfCookies(jwt, _requestContext.RequestChannel.ToString());

                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/LatePaymentsSet?$filter=AccountNo eq '{contractAccountId}' and Reconnect eq '{reconnectionFlag}'", Method.GET);
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
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<PostCancelMoveInMcfResponse> PostCancelMoveIn(CancelMoveInRequest request)
        {
            var response = new PostCancelMoveInMcfResponse();

            this._logger.LogInformation($"CreateCancelMoveInForContractId({nameof(request)}: {request.ToJson()})");

            try
            {
                var config = this._coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var restRequest = new RestRequest($"/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/CancelMoveIn?ContractID='{request.ContractId}'", Method.POST);

                restRequest.AddMcfRequestHeaders();
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("ContentType", "application/json");
                restRequest.AddHeader("Accept", "application/json");
                var cookies = restUtility.GetMcfCookies(_requestContext.JWT, _requestContext.ToString());
                restRequest.AddCookies(cookies.Result);

                // executing request and response
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var cancellationTokenSource = new CancellationTokenSource();
                var restResponse = await client.ExecuteTaskAsync(restRequest, cancellationTokenSource.Token);

                var mcfResponse = JsonConvert.DeserializeObject<McfResponse<PostCancelMoveInMcfResponse>>(restResponse.Content);
                if (mcfResponse.Error != null && mcfResponse.Result == null)
                {
                    var errorMessage = mcfResponse.Error.Message?.Value;
                    _logger.LogError(errorMessage);
                    response.Success = false;
                    response.StatusMessage = $"Cancellation Error - {restResponse.StatusCode}: {errorMessage}";
                }
                else
                {
                    response.Success = true;
                    response.StatusMessage = $"Cancellation Successful.";
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{e.Message} for {nameof(request)}: {request.ToJson(Formatting.None)}");
                throw;
            }

            return response;
        }

        /// <inheritdoc />
        public MoveInResponse PostPriorMoveIn(CreateMoveInRequest request, string jwt)
        {
            var config = _coreOptions.Configuration;
            _logger.LogInformation($"PostMoveIn(jwt, {nameof(request)}: {request}");
            var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
            var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
            var cookies = restUtility.GetMcfCookies(jwt, _requestContext.ToString());
            var body = JsonConvert.SerializeObject(request);

            var restRequest = new RestRequest("/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/ContractItems", Method.POST);
            restRequest.AddCookies(cookies.Result);
            restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
            restRequest.AddHeader("ContentType", "application/json");
            restRequest.AddHeader("Accept", "application/json");
            restRequest.AddParameter("application/json", body, ParameterType.RequestBody);

            _logger.LogInformation("Making MCF call");

            var response = client.Execute(restRequest);
            var mcfResponse = JsonConvert.DeserializeObject<McfResponse<MoveInResponse>>(response.Content);

            if (mcfResponse.Error != null)
            {
                var errorMsg = mcfResponse.Error.Message.Value;
                _logger.LogError(errorMsg);
                throw new BadRequestException(errorMsg);

            }

            return mcfResponse.Result;
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
                SetMcfAnonCredentials(ref mcfUserName, ref mcfUserPassword, customerAnonServiceUserName, customerAnonServiceUserPassword);
                restRequest.AddBasicCredentials(mcfUserName, mcfUserPassword);
                restRequest.AddMcfRequestHeaders();

                restRequest.AddJsonBody<CreateBusinesspartnerMcfRequest>(request);

                _logger.LogInformation("Making MCF call");

                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var cancellationTokenSource = new CancellationTokenSource();
                var restResponse = await client.ExecuteTaskAsync(restRequest, cancellationTokenSource.Token);
                if (!restResponse.IsSuccessful)
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
                throw;
            }

            return response;
        }
        #endregion

        #region Identifiers
        /// <inheritdoc />
        public McfResponse<McfResponseResults<BpIdentifier>> GetAllIdentifiers(string bpId)
        {
            McfResponse<McfResponseResults<BpIdentifier>> response;

            try
            {
                _logger.LogInformation($"GetAllIdentifiers({nameof(bpId)}: {bpId}); {_requestContext.ToJson()})");
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);

                var url = $"sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/Accounts('{bpId}')/Identifier";
                var restRequest = new RestRequest(url, Method.GET);

                if (string.IsNullOrWhiteSpace(_requestContext.JWT))
                {
                    // Add anon bypass auth
                    var mcfUserName = string.Empty;
                    var mcfUserPassword = string.Empty;
                    SetMcfAnonCredentials(ref mcfUserName, ref mcfUserPassword, customerAnonServiceUserName, customerAnonServiceUserPassword);
                    restRequest.AddBasicCredentials(mcfUserName, mcfUserPassword);
                }
                else
                {
                    var cookies = restUtility.GetMcfCookies(_requestContext.JWT, _requestContext.RequestChannel.ToString()).Result;
                    restRequest.AddCookies(cookies);
                }
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("Accept", "application/json");

                _logger.LogInformation("Making MCF call");
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);

                response = JsonConvert.DeserializeObject<McfResponse<McfResponseResults<BpIdentifier>>>(restResponse.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to get all identifiers {{{bpId}}}");
                throw;
            }

            return response;
        }

        /// <inheritdoc />
        public McfStatusCodeResponse<BpIdentifier> CreateIdentifier(BpIdentifier identifier)
        {
            McfStatusCodeResponse<BpIdentifier> response;
            var paramLogInfo = identifier.ToJsonNoPii();

            try
            {
                _logger.LogInformation($"CreateIdentifier({nameof(identifier)}: {paramLogInfo}); {_requestContext.ToJson()})");
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);

                var url = "/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/IdentifierSet";
                var restRequest = new RestRequest(url, Method.POST);

                if (string.IsNullOrWhiteSpace(_requestContext.JWT))
                {
                    // Add anon bypass auth
                    var mcfUserName = string.Empty;
                    var mcfUserPassword = string.Empty;
                    SetMcfAnonCredentials(ref mcfUserName, ref mcfUserPassword, customerAnonServiceUserName, customerAnonServiceUserPassword);
                    restRequest.AddBasicCredentials(mcfUserName, mcfUserPassword);
                }
                else
                {
                    var cookies = restUtility.GetMcfCookies(_requestContext.JWT, _requestContext.RequestChannel.ToString()).Result;
                    restRequest.AddCookies(cookies);
                }

                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("ContentType", "application/json");
                restRequest.AddHeader("Accept", "application/json");
                var requestBody = identifier.ToJson(Formatting.None);
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                _logger.LogInformation("Making MCF call");
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);

                response = JsonConvert.DeserializeObject<McfStatusCodeResponse<BpIdentifier>>(restResponse.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"CreateIdentifier({paramLogInfo}) failed");
                throw;
            }

            return response;
        }

        /// <inheritdoc />
        public McfStatusCodeResponse UpdateIdentifier(BpIdentifier identifier)
        {
            var response = new McfStatusCodeResponse();
            var paramLogInfo = identifier.ToJsonNoPii();

            try
            {
                _logger.LogInformation($"UpdateIdentifier({nameof(identifier)}: {paramLogInfo}); {_requestContext.ToJson()})");
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);

                _logger.LogInformation("Getting resource primary key to update");
                var allIds = GetAllIdentifiers(identifier.AccountId);
                var primaryKeyIdentifier = allIds?.Result?.Results.FirstOrDefault(x => x.IdentifierType == identifier.IdentifierType);

                _logger.LogInformation("Creating update request");
                var url = "/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/IdentifierSet(" +
                          $"AccountID='{identifier.AccountId}',Identifiertype='{identifier.IdentifierType}',Identifierno='{primaryKeyIdentifier?.IdentifierNo}')";
                var restRequest = new RestRequest(url, Method.PUT);

                var cookies = restUtility.GetMcfCookies(_requestContext.JWT, _requestContext.RequestChannel.ToString()).Result;
                restRequest.AddCookies(cookies);

                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("ContentType", "application/json");
                restRequest.AddHeader("Accept", "application/json");
                var requestBody = identifier.ToJson(Formatting.None);
                restRequest.AddParameter("application/json", requestBody, ParameterType.RequestBody);

                _logger.LogInformation("Making MCF call");
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);

                if (!restResponse.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<McfStatusCodeResponse>(restResponse.Content);
                }
                response.HttpStatusCode = restResponse.StatusCode;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"UpdateIdentifier({paramLogInfo}) failed");
                throw;
            }

            return response;
        }

        /// <inheritdoc />
        public McfStatusCodeResponse DeleteIdentifier(BpIdentifier identifier)
        {
            var response = new McfStatusCodeResponse();
            var paramLogInfo = identifier.ToJsonNoPii();

            try
            {
                _logger.LogInformation($"DeleteIdentifier({nameof(identifier)}: {paramLogInfo}); {_requestContext.ToJson()})");
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);

                var url = "/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/IdentifierSet(" +
                          $"AccountID='{identifier.AccountId}',Identifiertype='{identifier.IdentifierType}',Identifierno='{identifier.IdentifierNo.ToUpper()}')";
                var restRequest = new RestRequest(url, Method.DELETE);

                var cookies = restUtility.GetMcfCookies(_requestContext.JWT, _requestContext.RequestChannel.ToString()).Result;
                restRequest.AddCookies(cookies);

                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("ContentType", "application/json");
                restRequest.AddHeader("Accept", "application/json");

                _logger.LogInformation("Making MCF call");
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);

                if (!restResponse.IsSuccessful)
                {
                    response = JsonConvert.DeserializeObject<McfStatusCodeResponse>(restResponse.Content);
                }
                response.HttpStatusCode = restResponse.StatusCode;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"DeleteIdentifier({paramLogInfo}) failed");
                throw;
            }

            return response;
        }
        #endregion

        #region Bp Relationships
        /// <summary>
        /// Gets Bp relation ships for a given bp id
        /// </summary>
        /// <param name="bpId"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task<BpRelationshipsMcfResponse> GetBprelationships(string bpId, string jwt)
        {
            BpRelationshipsMcfResponse mcfResponse = null;

            try
            {
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var cookies = restUtility.GetMcfCookies(jwt, _requestContext.ToString());//TODO make the underlying call async
                var request = new RestRequest($"/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/Accounts('{bpId}')/Relationships", Method.GET);


                request.AddCookies(cookies.Result);
                request.AddMcfRequestHeaders();

                _logger.LogInformation("Making MCF call");

                var cancellationTokenSource = new CancellationTokenSource();
                var restResponse = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);
                if (!restResponse.IsSuccessful)
                {
                    HandleErrorResponse(bpId, restResponse);
                }

                var resp = JsonConvert.DeserializeObject<McfResponse<BpRelationshipsMcfResponse>>(restResponse.Content);

                mcfResponse = resp.Result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return mcfResponse;
        }

        /// <summary>
        /// Gets Bp Relationships for tenant bp using serv account
        /// </summary>
        /// <param name="tenantBp"></param>
        /// <returns></returns>
        public async Task<BpRelationshipsMcfResponse> GetBprelationships(string tenantBp)
        {
            BpRelationshipsMcfResponse mcfResponse = null;

            try
            {
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);

                var request = new RestRequest($"/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/Accounts('{tenantBp}')/Relationships", Method.GET);
                
                // Add tenant service auth
                var mcfUserName = string.Empty;
                var mcfUserPassword = string.Empty;
                SetMcfAnonCredentials(ref mcfUserName, ref mcfUserPassword, lrdServiceUserName,lrdServiceUserPassword);
                request.AddBasicCredentials(mcfUserName, mcfUserPassword);
                request.AddMcfRequestHeaders();

                _logger.LogInformation("Making MCF call");

                var cancellationTokenSource = new CancellationTokenSource();
                var restResponse = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);
                if (!restResponse.IsSuccessful || restResponse.ContentType.Equals("text/html; charset=utf-8"))
                {
                    HandleErrorResponse(tenantBp, restResponse);
                }

                var resp = JsonConvert.DeserializeObject<McfResponse<BpRelationshipsMcfResponse>>(restResponse.Content);

                mcfResponse = resp.Result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return mcfResponse;
        }


        /// <summary>
        /// /Creates Bp Relation ships
        /// </summary>
        /// <param name="jwt"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool CreateBpRelationship(string jwt, BpRelationshipRequest request, string tenantBpId)
        {


            try
            {
                var requestBody = request.ToJson(Formatting.None);
                _logger.LogInformation($"CreateBpRelationship(jwt, {nameof(request)}: {requestBody})");
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                //var cookies = restUtility.GetMcfCookies(jwt, _requestContext.ToString()).Result;

                string url = $"sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/Accounts('{request.AccountID1}')/Relationships";
                var restRequest = new RestRequest(url, Method.POST);

                if(!string.IsNullOrEmpty(tenantBpId))
                {
                    url = $"sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/Accounts('{tenantBpId}')/Relationships";
                    restRequest = new RestRequest(url, Method.POST);
                    // Add tenant service auth
                    var mcfUserName = string.Empty;
                    var mcfUserPassword = string.Empty;
                    SetMcfAnonCredentials(ref mcfUserName, ref mcfUserPassword, lrdServiceUserName, lrdServiceUserPassword);
                    restRequest.AddBasicCredentials(mcfUserName, mcfUserPassword);
                    restRequest.AddMcfRequestHeaders();
                    request.AccountID1 = tenantBpId;                  
                }
                else
                {
                    var cookies = restUtility.GetMcfCookies(jwt, _requestContext.ToString()).Result;
                    restRequest.AddCookies(cookies);
                    restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                    restRequest.AddHeader("ContentType", "application/json");
                    restRequest.AddHeader("Accept", "application/json");
                }

                restRequest.AddJsonBody<BpRelationshipRequest>(request);

                _logger.LogInformation("Making MCF call");
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);

                var mcfResponse = JsonConvert.DeserializeObject<McfResponse<McfResponseResult<BpRelationshipResponse>>>(restResponse.Content);
                if (mcfResponse.Error != null)
                {
                    var errorMsg = mcfResponse.Error.Message.Value;
                    _logger.LogError(errorMsg);
                    throw new BadRequestException(errorMsg);

                }
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }


        }
        /// <summary>
        /// update business partner relationship.
        /// </summary>
        /// <param name="jwt">Java Web Token for authentication</param>
        /// <param name="request">Request object</param>
        /// <returns>Response object</returns>
        /// <remarks>
        /// OData URI:
        /// PUT /sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/RelationshipsSet(AccountID1='1200000047',AccountID2='1200000806',Relationshipcategory='ZCOCU') 
        ///</remarks>
        public BpRelationshipUpdateResponse UpdateBusinessPartnerRelationship(BpRelationshipUpdateRequest request, string jwt)
        {
            BpRelationshipUpdateResponse response = new BpRelationshipUpdateResponse();

            try
            {
                _logger.LogInformation($"UpdateBusinessPartnerRelationship(jwt, {nameof(request)}: {request.ToJson()})");


                var mcfBprelationUpdateRequest = MaptoMcfBpUpdateRequest(request);
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
               
                // URL for updating BP relationship
                //string url = $"{config.McfEndpoint}/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/RelationshipsSet(AccountID1='{request.AccountID1}',AccountID2='{request.AccountID2}',Relationshipcategory='{request.Relationshipcategory}')";
                string url = $"/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/RelationshipsSet(AccountID1='{mcfBprelationUpdateRequest.AccountID1}',AccountID2='{mcfBprelationUpdateRequest.AccountID2}',Relationshipcategory='{mcfBprelationUpdateRequest.Relationshipcategory}')";
                var restRequest = new RestRequest(url, Method.PUT);
                restRequest.AddMcfRequestHeaders();
                var body = JsonConvert.SerializeObject(mcfBprelationUpdateRequest);
                restRequest.AddParameter("application/json", body, ParameterType.RequestBody);
                if (request.TenantBpId > 0)
                {
                    var mcfUserName = string.Empty;
                    var mcfUserPassword = string.Empty;
                    SetMcfAnonCredentials(ref mcfUserName, ref mcfUserPassword, lrdServiceUserName, lrdServiceUserPassword);
                    restRequest.AddBasicCredentials(mcfUserName, mcfUserPassword);
                }
                else
                {
                    var cookies = restUtility.GetMcfCookies(jwt, _requestContext.ToString()).Result;
                    restRequest.AddCookies(cookies);
                }
                _logger.LogInformation("Making MCF call");
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);
                if (!restResponse.IsSuccessful)
                {
                    var errorMsg = restResponse.Content;
                    _logger.LogError(errorMsg);
                    throw new BadRequestException(errorMsg);
                }

                response.status_code = restResponse.StatusCode.ToString();
                response.status_reason = restResponse.StatusDescription;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return response;
        }

        /// <summary>
        /// delete the business partner relationshipo
        /// </summary>
        /// <param name="request"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task<BpRelationshipUpdateResponse> DeleteBusinessPartnerRelationship(BpRelationshipUpdateRequest request, string jwt)
        {
            BpRelationshipUpdateResponse response = new BpRelationshipUpdateResponse();

            try
            {
                _logger.LogInformation($"UpdateBusinessPartnerRelationship(jwt, {nameof(request)}: {request.ToJson()})");
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var cookies = restUtility.GetMcfCookies(jwt, _requestChannel.ToString()).Result;

                // URL for updating BP relationship
                string url = $"{config.McfEndpoint}/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/RelationshipsSet(AccountID1='{request.AccountID1}',AccountID2='{request.AccountID2}',Relationshipcategory='{request.Relationshipcategory}')";


                var restRequest = new RestRequest(url, Method.DELETE);
                restRequest.AddCookies(cookies);
                restRequest.AddMcfRequestHeaders();

                //restRequest.AddJsonBody<BpRelationshipUpdateRequest>(request);

                _logger.LogInformation("Making MCF call");

                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var cancellationTokenSource = new CancellationTokenSource();
                var restResponse = await client.ExecuteTaskAsync(restRequest, cancellationTokenSource.Token);

                if (!restResponse.IsSuccessful)
                {
                    var errorMsg = restResponse.Content;
                    _logger.LogError(errorMsg);
                    throw new BadRequestException(errorMsg);
                }

                response.status_code = restResponse.StatusCode.ToString();
                response.status_reason = restResponse.StatusDescription;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return response;
        }
        #endregion

        #region Stop or Move out
        /// <inheritdoc />
        public McfResponse<GetContractItemMcfResponse> StopService(long contractItemId, long premiseId, DateTimeOffset moveoutDate)
        {
            #region Move - Out Reason Codes 
            /*
            BANK Bankruptcy
            CRED Credit/ Prior Obligation
            CREO Credit / Other
            DECE Deceased
            ENOA End Owner Allocation
            ERRO Assigned in Error
            FCLO Foreclosure
            FORC Forced out
            MOVE Moved
            REAS Reassigned
            SNLA Service No longer available at this location
            SSOF Seasonal Shut off
            WEBM Force Out from Web
            WEBO Move Out from Web 
            */
            #endregion
            const string EndReasonCode = "WEBO";


            var config = _coreOptions.Configuration;
            var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
            var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
            var cookies = restUtility.GetMcfCookies(_requestContext.JWT, _requestContext.RequestChannel.ToString());

            var restRequest = new RestRequest($"/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/MoveOut?MoveOutDate=datetime'{moveoutDate.ToString(McfDateFormat)}'" +
                $"&ContractID='{contractItemId}'&ContEndReason='{EndReasonCode}'&Premise='{premiseId}'", Method.POST);
            restRequest.AddCookies(cookies.Result);
            restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
            restRequest.AddHeader("ContentType", "application/json");
            restRequest.AddHeader("Accept", "application/json");

            _logger.LogInformation($"Making MCF call: {restRequest.Resource}");

            var response = client.Execute(restRequest);
            var mcfResponse = JsonConvert.DeserializeObject<McfResponse<GetContractItemMcfResponse>>(response.Content);

            if (mcfResponse.Error != null)
            {
                _logger.LogError($"Error from StopService mcf call: contractItemId: {contractItemId}, premiseId: {premiseId}, moveoutDate: {moveoutDate}, code: {mcfResponse.Error.Code}, message: {mcfResponse.Error.Message.Value}");
            }

            return mcfResponse;
        }
        public MoveInResponse PostMoveIn(CreateMoveInRequest request, string jwt)
        {
            var config = _coreOptions.Configuration;
            _logger.LogInformation($"PostMoveIn(jwt, {nameof(request)}: {request}");
            var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
            var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
            var cookies = restUtility.GetMcfCookies(jwt, _requestContext.RequestChannel.ToString());
            var body = JsonConvert.SerializeObject(request);

            var restRequest = new RestRequest("/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/ContractItems", Method.POST);
            restRequest.AddCookies(cookies.Result);
            restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
            restRequest.AddHeader("ContentType", "application/json");
            restRequest.AddHeader("Accept", "application/json");
            restRequest.AddParameter("application/json", body, ParameterType.RequestBody);

            _logger.LogInformation("Making MCF call");

            var response = client.Execute(restRequest);
            var mcfResponse = JsonConvert.DeserializeObject<McfResponse<MoveInResponse>>(response.Content);

            if (mcfResponse.Error != null)
            {
                var errorMsg = mcfResponse.Error.Message.Value;
                _logger.LogError(errorMsg);
                throw new BadRequestException(errorMsg);

            }

            return mcfResponse.Result;
        }


        public async Task<McfResponse<McfResponseResults<PremisesSet>>> GetPremises(string bpId)
        {
            McfResponse<McfResponseResults<PremisesSet>> response;

            try
            {
                _logger.LogInformation($"GetAllIdentifiers({nameof(bpId)}: {bpId}); {_requestContext.ToJson()})");
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);

                var url = $"/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/Premises('{bpId}')?$expand=Installations&$format=json";
                var restRequest = new RestRequest(url, Method.GET);

                var cookies = restUtility.GetMcfCookies(_requestContext.JWT, _requestContext.RequestChannel.ToString()).Result;
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("Accept", "application/json");

                _logger.LogInformation("Making MCF call");
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = await client.ExecuteTaskAsync(restRequest);

                response = JsonConvert.DeserializeObject<McfResponse<McfResponseResults<PremisesSet>>>(restResponse.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to get all identifiers {{{bpId}}}");
                throw;
            }

            return response;
        }

        /// <inheritdoc />
        public async Task<McfResponse<McfResponseResults<OwnerAccountsSet>>> GetOwnerAccounts(string bpId)
        {
            McfResponse<McfResponseResults<OwnerAccountsSet>> response;

            try
            {
                _logger.LogInformation($"GetAllIdentifiers({nameof(bpId)}: {bpId}); {_requestContext.ToJson()})");
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);

                var url = $"/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/Accounts('{bpId}')/OwnerAccounts?$expand=OwnerContractAccount/OwnerPremise/OwnerPremiseProperty&$format=json";
                var restRequest = new RestRequest(url, Method.GET);

                var cookies = restUtility.GetMcfCookies(_requestContext.JWT, _requestContext.RequestChannel.ToString()).Result;
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("Accept", "application/json");

                _logger.LogInformation("Making MCF call");
                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = await client.ExecuteTaskAsync(restRequest);

                response = JsonConvert.DeserializeObject<McfResponse<McfResponseResults<OwnerAccountsSet>>>(restResponse.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to get all identifiers {{{bpId}}}");
                throw;
            }

            return response;
        }
        #endregion

        #region Private methods

        
        private void SetMcfAnonCredentials(ref string userName, ref string password, string userNamekey, string passwordKey)
        {
            _logger.LogInformation("SetMcfAnonCredentials()");
            var options = new CoreOptions(ServiceConfiguration.AppName);
            ICoreOptions _options = options;

            var isLocal = (_environment?.Equals("Development") ?? true) || string.IsNullOrEmpty(_environment);
            var mcfCustomerUserNameParamName = $"/CI/MS/{_environment}/MCF/{userNamekey}";
            var mcfCustomerUserPasswordParamName = $"/CI/MS/{_environment}/MCF/{passwordKey}";

            if (!isLocal)
            {
                // this is hardcoded to retrieve secure strings
                userName = _options.GetValueFromParameterStore(mcfCustomerUserNameParamName, true);
                password = _options.GetValueFromParameterStore(mcfCustomerUserPasswordParamName, true);
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

        private void HandleErrorResponse(string bpId, IRestResponse restResponse)
        {           
            var errormessage = $"Accounts('{bpId}')/Relationships was not successfull with Error {restResponse.Content}";
            _logger.LogError(errormessage);
            if (restResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new ResourceNotFoundException(errormessage);
            }
            else
            {

                throw new Exception(errormessage);
            }
        }

        private BpRelationshipUpdateMcfRequest MaptoMcfBpUpdateRequest(BpRelationshipUpdateRequest request)
        {
            var bpUpdaterelation = new BpRelationshipUpdateMcfRequest()
            {
                AccountID1 = request.AccountID1,
                AccountID2 = request.AccountID2,
                Defaultrelationship = request.Defaultrelationship,
                Differentiationtypevalue = request.Differentiationtypevalue,
                Message = request.Message,
                Relationshipcategory = request.Relationshipcategory,
                Relationshiptypenew = request.Relationshiptypenew,
                Validfromdate = request.Validfromdate,
                Validfromdatenew = request.Validfromdatenew,
                Validtodate = request.Validtodate,
                Validtodatenew = request.Validtodatenew,

            };
            return bpUpdaterelation;
        }

        #region Kept this for testing/debuging 
        /// <summary>
        /// Sends an Http Request via TCP client.
        /// This is to circumvent cases when HttpClient is not capable of parsing a response.
        /// An example is when the server replies with HttpStatusCode 204: in this case, 
        /// HttpClient throws an exception (HttpRequestException with HResult 0x80072f78.
        /// </summary>
        /// <param name="request">
        /// The request to be sent.
        /// It must be a complete request, with at least:
        /// * Full RequestUri
        /// * Method
        /// * All necessary headers
        /// </param>
        /// <returns>An McfResponseBase.</returns>
        private async Task<string> SendHttpRequestWithTcpClientAsync(HttpRequestMessage request)
        {
            var path = request.RequestUri.PathAndQuery.Replace("'", "%27");

            var httpMessage =
                $"{request.Method.Method.ToUpperInvariant()} {path} HTTP/1.1{CRLF}" +
                $"accept-encoding: gzip, deflate{CRLF}" +
                $"content-length: 0{CRLF}" +
                $"Connection: keep-alive{CRLF}" +
                $"X-Requested-With: XMLHttpRequest{CRLF}" +
                $"cache-control: no-cache{CRLF}" +
                $"Accept: */*{CRLF}" +
                $"Host: {request.RequestUri.Host}:{request.RequestUri.Port}{CRLF}";

            foreach (var header in request.Headers.Select(h => new KeyValuePair<string, string>(h.Key.ToLowerInvariant(), h.Value.First())))
            {
                if (!httpMessage.ToLowerInvariant().Contains(header.Key + ":"))
                {
                    httpMessage += $"{header.Key}:{header.Value}{CRLF}";
                }
            }
            httpMessage += CRLF;
            var response = "";
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(request.RequestUri.Host, request.RequestUri.Port);
                using (var stream = client.GetStream())
                {
                    if (request.RequestUri.Scheme == "https")
                    {
                        using (var sslStream = new SslStream(stream, false, AcceptAllServerCertificate, null))
                        {
                            await sslStream.AuthenticateAsClientAsync(this._coreOptions.Configuration.SecureMcfEndpoint, null, SslProtocols.Tls, false);
                            response = await SendRequestThroughStreamAsync(sslStream, httpMessage);
                        }
                    }
                    else
                    {
                        response = await SendRequestThroughStreamAsync(stream, httpMessage);
                    }
                }
            }
            var statusCode = (HttpStatusCode)int.Parse(response.Split(' ')[1]);
            return statusCode.ToString();
        }

        /// <summary>
        /// Inform SSL that we accept all server certificates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns>Always true.</returns>
        private static bool AcceptAllServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors) => true;

        /// <summary>
        /// Sends a message through a stream (for instance, a stream from a TCP Client), then immediately reads text lines through the stream until we receive an empty line.
        /// This is the standard for HTTP traffic.
        /// </summary>
        /// <param name="stream">The stream to send the message, then to read the response.</param>
        /// <param name="message">The message to be sent.</param>
        /// <returns>The lines that were read, each line separated by CRLF.</returns>
        private static async Task<string> SendRequestThroughStreamAsync(Stream stream, string message)
        {
            var response = new StringBuilder();
            try
            {
                using (var writer = new StreamWriter(stream))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        await writer.WriteAsync(message);
                        await writer.FlushAsync();
                        string line;
                        while ((line = await reader.ReadLineAsync()) != string.Empty)
                        {
                            response.Append(line);
                            response.Append(CRLF);
                        }
                    }
                }

                response.Append(CRLF);
            }
            catch (Exception e)
            {
                //_logger.LogInformation(e.Message);
                response.Clear();
                response.Append($"HTTP/1.1 500 Internal server error{CRLF}{CRLF}");
            }
            return response.ToString();
        }
        #endregion

        #endregion
    }
}