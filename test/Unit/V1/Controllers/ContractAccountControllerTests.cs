using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSE.Customer.Configuration;
using PSE.Customer.V1.Logic.Interfaces;

// ReSharper disable once CheckNamespace
namespace PSE.Customer.V1.Controllers.Tests.Unit
{
    [TestClass]
    public class ContractAccountControllerTests
    {

        private Mock<IOptions<AppSettings>> _appSettingsMock;
        private Mock<IDistributedCache> _distributedCacheMock;
        private Mock<ICustomerLogic> _customerLogicMock;

        [TestInitialize]
        public void Setup()
        {
            _appSettingsMock = new Mock<IOptions<AppSettings>>();
            _distributedCacheMock = new Mock<IDistributedCache>();
            _customerLogicMock = new Mock<ICustomerLogic>();
        }
    }
}