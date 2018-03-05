using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSE.Customer.Configuration;
using PSE.Customer.V1.Logic;
using PSE.Customer.Tests.Integration.TestObjects;
using PSE.WebAPI.Core.Configuration;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace PSE.Customer.V1.Controllers.Tests.Integration
{
    [TestClass]
    public class ContractAccountControllerE2ETests
    {
        private static AppSettings Settings => TestHelper.GetAppSettings();

        #region Test Helper Methods

        private static ILogger<T> GetLogger<T>()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(LogLevel.Trace, true);

            return loggerFactory.CreateLogger<T>();
        }

        private static IDistributedCache GetCache()
        {
            var options = Options.Create(new MemoryDistributedCacheOptions());

            return new MemoryDistributedCache(options);
        }

        private static CustomerLogic GetCustomerLogic()
        {
            var logger = GetLogger<CustomerLogic>();
            var appSettings = Options.Create(new AppSettings());
            var options = Options.Create(new MemoryCacheOptions());
            IDistributedCache cache = GetCache();
            var localCache = new MemoryCache(options);
            var coreOptions = new CoreOptions(ServiceConfiguration.AppName);

            return new CustomerLogic(cache, localCache, appSettings, logger, coreOptions);
        }

        #endregion Test Helper Methods
    }
}
