using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PSE.Customer.V1.Logic.Interfaces;
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
            _logger = logger;
            _logic = logic;
        }
    }
}
