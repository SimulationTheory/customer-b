using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PSE.Customer.Configuration;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Request;
using PSE.Customer.Configuration;
using PSE.WebAPI.Core.Exceptions;
using PSE.WebAPI.Core.Service;
using PSE.WebAPI.Core.Startup.Attributes;
using System;
using System.Threading.Tasks;
using PSE.Customer.V1.Repositories.Entities;
using PSE.WebAPI.Core.Service.Interfaces;

namespace PSE.Account.V1.Controllers
{

    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/private/account")]
    public class SyncController : PSEController
    {
        private readonly AppSettings _config;
        private readonly ILogger<SyncController> _logger;
        private readonly ICustomerLogic _customerSummaryLogic;
        private readonly IRequestContextAdapter _contextAdapter;
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncController"/> class.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="accountSummaryLogic">The account summary logic.</param>
        /// <exception cref="ArgumentNullException">
        /// appSettings
        /// or
        /// logger
        /// or
        /// accountSummaryLogic
        /// </exception>
        public SyncController(
            IOptions<AppSettings> appSettings,
            ILogger<SyncController> logger,
            ICustomerLogic customerSummaryLogic,
            IRequestContextAdapter requestContextAdapter)
        {
            _config = (appSettings ?? throw new ArgumentNullException(nameof(appSettings))).Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _customerSummaryLogic = customerSummaryLogic ?? throw new ArgumentNullException(nameof(customerSummaryLogic));
            _contextAdapter = requestContextAdapter ?? throw new ArgumentNullException(nameof(requestContextAdapter));
        }

        /// <summary>
        /// Puts the customer data from MCF to Cassandra asynchronous.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(OkResult), 200)]
        [RequestChannelSkipValidation]
        [AllowAnonymous]
        [HttpPut("{bpId}/sync")]
        public async Task<IActionResult> PutSynchronizeCustomerAsync(long bpId)
        {
            _logger.LogInformation($"PutSynchronizeCustomerAsync()");

            IActionResult result = BadRequest(ModelState);

            try
            {
                 var successfulUpdate = await _customerSummaryLogic.SyncCustomerByBpId(bpId);
                if (successfulUpdate)
                {

                    result = Ok();
                }
             
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                result = e.ToActionResult();
            }

            return result;
        }
    }
}