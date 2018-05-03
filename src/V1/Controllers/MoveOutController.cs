using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PSE.Customer.Configuration;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Exceptions;
using PSE.WebAPI.Core.Service;
using System;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Controllers
{
    /// <summary>
    /// API to facilitate customers moving, both new and existing customers
    /// </summary>
    /// <seealso cref="PSEController" />
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Authorize("ContractAccountValidator")]
    [Route("v{version:apiVersion}/customer")]
    public class MoveOutController : PSEController
    {
        private readonly AppSettings _config;
        private readonly ILogger<MoveInController> _logger;
        private readonly IMoveOutLogic _moveOutLogic;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveOutController" /> class.
        /// </summary>
        /// <param name="appSettings">application config information</param>
        /// <param name="logger">The logger.</param>
        /// <param name="moveOutLogic">The move out logic.</param>
        /// <exception cref="ArgumentNullException">logger
        /// or
        /// moveInLogic</exception>
        public MoveOutController(
            IOptions<AppSettings> appSettings,
            ILogger<MoveInController> logger,
            IMoveOutLogic moveOutLogic)
        {
            _config = (appSettings ?? throw new ArgumentNullException(nameof(appSettings))).Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moveOutLogic = moveOutLogic ?? throw new ArgumentNullException(nameof(moveOutLogic));
        }

        /// <summary>
        /// Stop service on the given contract account Id
        /// for the given installationIds.
        /// </summary>
        /// <remarks>
        /// Response includes a map of installationIds with status msgs.
        ///  List of status messages:
        ///   - Stop service request succeeded.
        ///   - InstallationId not found.
        ///   - InstallationId not on contractAccountId.
        /// </remarks>
        /// <param name="stopServiceRequest">The stop service request.</param>
        /// <returns></returns>
        /// <response code="200">Successfully stopped service.</response>
        /// <response code="401">Unauthorized.  Requires a valid JWT.</response>
        /// <response code="404">ContractAccountId not found or installationIds not found on contract account.</response>
        [AllowAnonymous]
        [HttpPost("moveout-stop-service/{contractAccountId}")]
        [ProducesResponseType(typeof(MoveOutStopServiceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostStopService([FromBody] MoveOutStopServiceRequest stopServiceRequest)
        {
            IActionResult result;
            _logger.LogInformation($"PostStopService: {JsonConvert.SerializeObject(stopServiceRequest, Formatting.Indented)}");

            try
            {
                var response = await _moveOutLogic.StopService(stopServiceRequest);

                if (response == null)
                {
                    result = NotFound();
                }
                else
                {
                    result = Ok(response);
                    _logger.LogInformation($"PostStopService: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in PostStopService: {JsonConvert.SerializeObject(stopServiceRequest, Formatting.Indented)}");
                result = e.ToActionResult();
            }

            return result;
        }
    }
}