using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSE.Customer.Configuration;
using PSE.WebAPI.Core.Configuration.Interfaces;

// ReSharper disable once CheckNamespace
namespace PSE.Customer.V1.Logic.Tests.Unit
{
    [TestClass]
    public class CustomerLogicTests
    {
        private Mock<IDistributedCache> _redisCacheMock;
        private Mock<IMemoryCache> _localCacheMock;
        private Mock<ILogger<CustomerLogic>> _loggerMock;
        private Mock<IOptions<AppSettings>> _appSettingsMock;
        private Mock<ICoreOptions> _coreOptionsMock;
       
         

        [TestInitialize]
        public void Setup()
        {
            _redisCacheMock = new Mock<IDistributedCache>();
            _localCacheMock = new Mock<IMemoryCache>();
            _loggerMock = new Mock<ILogger<CustomerLogic>>();
            _appSettingsMock = new Mock<IOptions<AppSettings>>();
            _coreOptionsMock = new Mock<ICoreOptions>();
        }
    }
}