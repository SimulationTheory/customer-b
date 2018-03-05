using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PSE.Customer.Configuration;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.WebAPI.Core.Configuration.Interfaces;

namespace PSE.Customer.V1.Logic
{
    public class CustomerLogic : ICustomerLogic
    {
        private readonly ILogger<CustomerLogic> _logger;
        private readonly IDistributedCache _redisCache;
        private readonly IMemoryCache _localCache;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ICoreOptions _options;

        public CustomerLogic(IDistributedCache redisCache, IMemoryCache localCache, IOptions<AppSettings> appSettings, ILogger<CustomerLogic> logger, ICoreOptions options)
        {
            _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
            _localCache = localCache ?? throw new ArgumentNullException(nameof(localCache));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
;            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options;
        }
    }
}