using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PSE.Customer.Extensions;
using PSE.Customer.V1.Clients.Address.Interfaces;
using PSE.Customer.V1.Clients.Device.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Logic.Extensions;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Exceptions.Types;
using PSE.WebAPI.Core.Service.Interfaces;

namespace PSE.Customer.V1.Logic
{
    /// <inheritdoc />
    public class MoveOutLogic : IMoveOutLogic
    {
        private readonly ILogger<MoveInLogic> _logger;
        private readonly IMcfClient _mcfClient;
        private static string coreLanguage = "EN";
        private readonly IAddressApi _addressApi;
        private readonly IDeviceApi _deviceApi;
        private readonly IRequestContextAdapter _requestContext;


        /// <summary>
        /// Initializes a new instance of the <see cref="MoveOutLogic"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mcfClient">The MCF client.</param>
        /// <param name="addressApi">The address API.</param>
        /// <param name="deviceApi">The device API.</param>
        /// <param name="requestContext">The request context.</param>
        /// <exception cref="ArgumentNullException">
        /// logger
        /// or
        /// mcfClient
        /// or
        /// addressApi
        /// or
        /// deviceApi
        /// or
        /// requestContext
        /// </exception>
        public MoveOutLogic(ILogger<MoveInLogic> logger, IMcfClient mcfClient, IAddressApi addressApi, IDeviceApi deviceApi, IRequestContextAdapter requestContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mcfClient = mcfClient ?? throw new ArgumentNullException(nameof(mcfClient));
            _addressApi = addressApi ?? throw new ArgumentNullException(nameof(addressApi));
            _deviceApi = deviceApi ?? throw new ArgumentNullException(nameof(deviceApi));
            _requestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
        }
    }
}