using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PSE.Customer.V1.Clients.Mcf.Enums;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.RestUtility.Core.Extensions;
using PSE.RestUtility.Core.Mcf;
using PSE.WebAPI.Core.Configuration.Interfaces;
using RestSharp;

namespace PSE.Customer.V1.Clients.Mcf
{
    public class McfClient : IMcfClient
    {
        private readonly ICoreOptions _coreOptions;
        private readonly ILogger<McfClient> _logger;

        public McfClient(ICoreOptions coreOptions, ILogger<McfClient> logger)
        {
            _coreOptions = coreOptions ?? throw new ArgumentNullException(nameof(coreOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(coreOptions));
        }

        public McfResponse<CreateAddressIndependantPhoneResponse> CreateBusinessPartnerLevelPhone(string jwt, CreateAddressIndependantPhoneRequest request)
        {
            McfResponse<CreateAddressIndependantPhoneResponse> response;

            try
            {
                var config = _coreOptions.Configuration;
                var restUtility = new RestUtility.Core.Utility(config.LoadBalancerUrl, config.RedisOptions);
                var cookies = restUtility.GetMcfCookies(jwt).Result;

                var restRequest = new RestRequest("sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/AccountAddressIndependentPhones", Method.POST);
                restRequest.AddCookies(cookies);
                restRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                restRequest.AddHeader("ContentType", "application/json");
                restRequest.AddHeader("Accept", "application/json");
                restRequest.AddParameter("application/json", JsonConvert.SerializeObject(request), ParameterType.RequestBody);

                var client = restUtility.GetRestClient(config.SecureMcfEndpoint);
                var restResponse = client.Execute(restRequest);

                response = JsonConvert.DeserializeObject<McfResponse<CreateAddressIndependantPhoneResponse>>(restResponse.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw e;
            }

            return response;
        }

        public void GetAddressIndependantContactInfo(IEnumerable<AddressIndependantContactInfoEnum> expand)
        {
            try
            {
            }
            catch (Exception e)
            {
            }
        }
    }
}