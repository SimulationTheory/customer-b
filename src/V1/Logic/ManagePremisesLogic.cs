using Microsoft.Extensions.Logging;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.WebAPI.Core.Service.Interfaces;

namespace PSE.Customer.V1.Logic
{
    /// <inheritdoc />
    public class ManagePremisesLogic : IManagePremisesLogic
    {
        private readonly IRequestContextAdapter _requestContext;
        private readonly ILogger<ManagePremisesLogic> _logger;
        private readonly IMcfClient _mcfClient;

        public ManagePremisesLogic(IRequestContextAdapter requestContext, ILogger<ManagePremisesLogic> logger, IMcfClient mcfClient)
        {
            _requestContext = requestContext;
            _logger = logger;
            _mcfClient = mcfClient;
        }
    }
}
