using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PSE.Customer.Extensions;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Logic.Extensions;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Exceptions.Types;
using PSE.WebAPI.Core.Service.Interfaces;

namespace PSE.Customer.V1.Logic
{
    /// <inheritdoc />
    public class ManagePremisesLogic : IManagePremisesLogic
    {
        private readonly IRequestContextAdapter _requestContext;
        private readonly ILogger<ManagePremisesLogic> _logger;
        private readonly IMcfClient _mcfClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagePremisesLogic"/> class.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mcfClient">The MCF client.</param>
        public ManagePremisesLogic(IRequestContextAdapter requestContext, ILogger<ManagePremisesLogic> logger, IMcfClient mcfClient)
        {
            _requestContext = requestContext;
            _logger = logger;
            _mcfClient = mcfClient;
        }

        /// <inheritdoc />
        public async Task<GetPremisesResponse> GetPremises(string bpId)
        {
            _logger.LogInformation($"GetPremises({nameof(bpId)} : {bpId})");

            try
            {
                var results = await _mcfClient.GetPremises(bpId);
                if (results.Error != null)
                {
                    if (results.Error.Message.Value.Contains("not found"))
                        return new GetPremisesResponse();
                    throw new InternalServerException(results.Error.ToJson());
                }

                return new GetPremisesResponse
                {
                    Premises = results.Result.Results.ToModels()
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get premises.");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<GetOwnerAccountsResponse> GetOwnerAccounts(string bpId)
        {
            _logger.LogInformation($"GetOwnerAccounts({nameof(bpId)} : {bpId})");

            try
            {
                var results = await _mcfClient.GetOwnerAccounts(bpId);
                if (results.Error != null)
                {
                    if (results.Error.Message.Value.Contains("No active properties"))
                        return new GetOwnerAccountsResponse();
                    throw new InternalServerException(results.Error.ToJson());
                }

                return new GetOwnerAccountsResponse
                {
                    OwnerAccounts = results.Result.Results.ToModels()
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get owner accounts.");
                throw;
            }
        }
    }
}
