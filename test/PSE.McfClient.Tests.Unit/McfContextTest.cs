using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using StackExchange.Redis;

namespace PSE.McfClient.Tests.Unit
{
    [TestClass]
    public partial class McfContextTest
    {
        Uri _serviceRoot;
        Uri _loadBalancer;
        Mock<ILogger> _logger;
        IDatabase _redis;
        Type _httpType;
        Type _restClientType;




        [TestInitialize]
        public void Initialize()
        {
            _serviceRoot = new Uri("http://serviceRoot");
            _loadBalancer = new Uri("http://loadBalancer");
            _logger = new Mock<ILogger>();
            _redis = new TestRedis();
            _httpType = typeof(TestHttpClient);
            _restClientType = typeof(TestRestClient);
        }

        public McfContext CreateMcfContext() => new McfContext(
            _serviceRoot,
            _loadBalancer,
            true,
            _logger?.Object,
            _redis,
            _httpType,
            _restClientType);

        [TestMethod]
        public void McfContextTest_Constructor_Null_ServiceRoot_Should_Throw()
        {
            // Arrange
            _serviceRoot = null;
            McfContext context = null;

            // Act
            Action action = () => context = CreateMcfContext();

            // Assert
            context.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("serviceRoot");
        }

        [TestMethod]
        public void McfContextTest_Constructor_Null_LoadBalancer_Should_Throw()
        {
            // Arrange
            _loadBalancer = null;
            McfContext context = null;

            // Act
            Action action = () => context = CreateMcfContext();

            // Assert
            context.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("loadBalancerUri");
        }

        [TestMethod]
        public void McfContextTest_Constructor_Null_Logger_Should_Throw()
        {
            // Arrange
            _logger  = null;
            McfContext context = null;

            // Act
            Action action = () => context = CreateMcfContext();

            // Assert
            context.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("logger");
        }

        [TestMethod]
        public void McfContextTest_Constructor_Null_Redis_Should_Throw()
        {
            // Arrange
            _redis = null;
            McfContext context = null;

            // Act
            Action action = () => context = CreateMcfContext();

            // Assert
            context.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("redis");
        }

        [TestMethod]
        public async Task McfContextTest_GetAsync_Existing_Account_Should_Succeed()
        {
            // Arrange
            var context = CreateMcfContext();

            // Act
            var result = await context.GetAsync<PaymentArrangement>($"/SOME/OP/{TestHttpClient.EligibleAccountId}", "jwt");

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<PaymentArrangement>();
            var compareResult = (new CompareLogic()).Compare(result, TestHttpClient.EligiblePaymentArrangement);
            Assert.IsTrue(compareResult.AreEqual);
        }


        [TestMethod]
        public async Task McfContextTest_PostAsync_Existing_Account_Should_Succeed()
        {
            // Arrange
            var context = CreateMcfContext();

            // Act
            var result = await context.PostAsync<PaymentArrangement>($"/SOME/OP/{TestHttpClient.EligibleAccountId}", TestHttpClient.EligiblePaymentArrangement, "jwt");

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<McfResponse<PaymentArrangement>>();
            var compareResult = (new CompareLogic()).Compare(result.Value, TestHttpClient.EligiblePaymentArrangement);
            Assert.IsTrue(compareResult.AreEqual);
        }
    }
}
