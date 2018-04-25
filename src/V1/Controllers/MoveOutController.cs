using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PSE.Customer.Configuration;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Exceptions;
using PSE.WebAPI.Core.Service;
using System;

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
        /// Stop service on the given contract account Id.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <response code="200">Successfully retrieved invalid movein dates.</response>
        /// <response code="401">Unauthorized.  Requires a valid JWT.</response>
        /// <response code="404">ContractAccountId not found.</response>
        [HttpPost("moveout-stop-service/{contractAccountId}")]
        [ProducesResponseType(typeof(MoveOutStopServiceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult PostStopService([FromBody] MoveOutStopServiceRequest stopServiceRequest)
        {
            IActionResult result;

            try
            {
                var response = new MoveOutStopServiceResponse() { WarmHomeFund = true };

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