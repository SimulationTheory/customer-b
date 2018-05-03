using Microsoft.Extensions.Logging;
using PSE.Customer.V1.Clients.Address.Interfaces;
using PSE.Customer.V1.Clients.Device.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Logic
{
    /// <inheritdoc />
    public class MoveOutLogic : IMoveOutLogic
    {
        private readonly ILogger<MoveInLogic> _logger;
        private readonly IMcfClient _mcfClient;
        private static string coreLanguage = "EN";
        private readonly IAddressApi _addressApi;
        private readonly IAccountApi _accountApi;
        private readonly IDeviceApi _deviceApi;
        private readonly IRequestContextAdapter _requestContext;


        /// <summary>
        /// Initializes a new instance of the <see cref="MoveOutLogic" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mcfClient">The MCF client.</param>
        /// <param name="addressApi">The address API.</param>
        /// <param name="accountApi">The device API.</param>
        /// <param name="deviceApi">The device API.</param>
        /// <param name="requestContext">The request context.</param>
        /// <exception cref="ArgumentNullException">logger
        /// or
        /// mcfClient
        /// or
        /// addressApi
        /// or
        /// deviceApi
        /// or
        /// requestContext</exception>
        public MoveOutLogic(
            ILogger<MoveInLogic> logger, 
            IMcfClient mcfClient, 
            IAddressApi addressApi, 
            IAccountApi accountApi, 
            IDeviceApi deviceApi, 
            IRequestContextAdapter requestContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mcfClient = mcfClient ?? throw new ArgumentNullException(nameof(mcfClient));
            _addressApi = addressApi ?? throw new ArgumentNullException(nameof(addressApi));
            _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
            _deviceApi = deviceApi ?? throw new ArgumentNullException(nameof(deviceApi));
            _requestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
        }

        /// <inheritdoc />
        public async Task<MoveOutStopServiceResponse> StopService(MoveOutStopServiceRequest stopServiceRequest)
        {
            const int BillDueDays = 15;
            var response = new MoveOutStopServiceResponse();
            response.Status = new Dictionary<long, string>();

            // ContractItem and installation have a 1 to 1 relationship.
            // We need to convert installation Ids from the caller to contractItem Ids.

            if (stopServiceRequest.InstallationIds != null && stopServiceRequest.InstallationIds.Count > 0)
            {
                // Get all ContractItems for the contract account id
                var contractItems = (await _accountApi.GetContractItems(stopServiceRequest.ContractAccountId)).ContractItems;

                if (contractItems.Count > 0)
                {
                    // Success is used to avoid calling Warm Home Fund endpoint if not needed
                    bool success = false;

                    // Check each installation passed in the request
                    foreach (long installationId in stopServiceRequest.InstallationIds)
                    {
                        var installation = await _deviceApi.GetInstallationDetail(installationId);
                        if (installation.Data != null)
                        {
                            // Get the contract info for that installlation
                            var contractItem = contractItems.Find(ci => ci.PointOfDeliveryGuid.ToLower() == installation.Data.Installation.InstallationGuid.ToLower());

                            // If they match on installation Guid attempt stop service
                            if (contractItem != null)
                            {
                                var mcfResponse = _mcfClient.StopService(contractItem.ContractId, contractItem.PremiseId, stopServiceRequest.MoveOutDate);
                                if (mcfResponse.Error == null)
                                {
                                    response.Status.Add(installationId, "Stop service request succeeded.");
                                    success = true;
                                }
                                else
                                {
                                    // Stop service mcf request failed
                                    response.Status.Add(installationId, $"{mcfResponse.Error.Code}: {mcfResponse.Error.Message.Value}.");
                                }
                            }
                            else
                            {
                                // Contract match by installationGuid failed
                                response.Status.Add(installationId, $"InstallationId not found for this contractAccountId.");
                            }
                        }
                        else
                        {
                            // InstallationId lookup failed
                            response.Status.Add(installationId, "InstallationId not found.");
                        }
                    }

                    // Add warm home fund amount & dates from request data if any succeeded
                    if (success)
                    {
                        response.FinalBillDate = stopServiceRequest.MoveOutDate;
                        response.FinalBillDueDate = stopServiceRequest.MoveOutDate.AddDays(BillDueDays);

                        // Get warmHomeFundAmount from account service
                        var contractDetails = await _accountApi.GetContractAccountDetails(stopServiceRequest.ContractAccountId);
                        if (contractDetails != null)
                        {
                            response.WarmHomeFund = contractDetails.WarmHomeFundAmount;
                        }
                        else
                        {
                            // Since actual Stop Service calls have succeeded, if this fails don't throw error, just log.  
                            // An alternative would be to go back and cancel each stop however Cancel function is out of scope for this project per Lucas.
                            _logger.LogError($"Error in StopService.  Unable to get WarmHomeFund value from account service for contractAccountId: {stopServiceRequest.ContractAccountId}.");
                        }
                    }
                }
                else
                {
                    // ContractItems lookup for contractAccountId failed - 404?
                    response = null;
                }
            }
            else
            {
                // Null or empty installationId list - 404?
                response = null;
            }

            return response;
        }
    }
}