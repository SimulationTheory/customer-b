using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Exceptions;
using PSE.WebAPI.Core.Service;

namespace PSE.Customer.V1.Controllers
{
    /// <summary>
    /// API for managing customers with multiple premises and landlords managing multiple premises
    /// </summary>
    /// <seealso cref="PSEController" />
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Authorize("ContractAccountValidator")]
    [Route("v{version:apiVersion}/customer")]
    public class ManagePremisesController : PSEController
    {
        private readonly ILogger<ManagePremisesController> _logger;
        private readonly IManagePremisesLogic _logic;

        public ManagePremisesController(
            ILogger<ManagePremisesController> logger,
            IManagePremisesLogic logic)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
        }

        /// <summary>
        /// Gets all of the premises, which has a list of properties for a business partner from MCF.
        /// </summary>
        /// <param name="bpId"></param>
        [HttpGet("premises")]
        [ProducesResponseType(typeof(GetPremisesResponse), 200)]
        public async Task<IActionResult> GetPremises(string bpId)
        {
            IActionResult result;

            try
            {
                _logger.LogInformation($"GetPremises({nameof(bpId)} : {bpId})");
                var response = await _logic.GetPremises(bpId);
                result = Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                result = e.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Gets all of the owner accounts for landlords, which has a list of properties for a business partner
        /// in MCF.
        /// </summary>
        /// <param name="bpId">The business partner ID</param>
        [HttpGet("owner-accounts")]
        [ProducesResponseType(typeof(GetOwnerAccountsResponse), 200)]
        public async Task<IActionResult> GetOwnerAccounts(string bpId)
        {
            IActionResult result;

            try
            {
                _logger.LogInformation($"GetOwnerAccounts({nameof(bpId)} : {bpId})");
                var response = await _logic.GetOwnerAccounts(bpId);
                result = Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                result = e.ToActionResult();
            }

            return result;
        }

    }
}
