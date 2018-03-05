using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PSE.Customer.Configuration;
using PSE.WebAPI.Core.Service;

namespace PSE.Customer.V1.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/account/{contractAccountId}/customer")]
    public class ContractAccountController : PSEController
    {
        private readonly AppSettings _config;
        private readonly IDistributedCache _cache;
        private readonly ILogger<ContractAccountController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractAccountController"/> class.
        /// </summary>
        /// <param name="appSettings"></param>
        /// <param name="cache"></param>
        /// <param name="logger"></param>
        public ContractAccountController(IOptions<AppSettings> appSettings, IDistributedCache cache, ILogger<ContractAccountController> logger)
        {
            _config = (appSettings ?? throw new ArgumentNullException(nameof(appSettings))).Value;
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}