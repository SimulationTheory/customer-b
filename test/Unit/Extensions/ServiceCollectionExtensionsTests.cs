using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSE.Test.Core;

// ReSharper disable once CheckNamespace
namespace PSE.Customer.Extensions.Tests.Unit
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestClass]
        public class AddRespositories
        {
            #region Test Helper Methods

            private void TestNullParameters(ServiceCollection services, ILogger logger, string expectedParamName)
            {
                try
                {
                    // test target method
                    AutoMapper.Mapper.Reset();
                    ServiceCollectionExtensions.AddRepositories(services, logger);

                    Assert.Fail("The expected ArgumentNullException was not thrown.");
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine(ex);

                    // assertions
                    Assert.AreEqual(expectedParamName, ex.ParamName);
                }
            }

            #endregion Test Helper Methods

            [TestMethod]
            public void AddRepositories_NullServicesThrows_Test()
            {
                // init vars
                const ServiceCollection services = null;
                var logger = CoreHelper.GetLogger<Startup>();
                const string expectedParamName = nameof(services);

                // test target month
                TestNullParameters(services, logger, expectedParamName);
            }

            [TestMethod]
            public void AddRepositories_NullLoggerThrows_Test()
            {
                // init vars
                var services = new ServiceCollection();
                const ILogger logger = null;
                const string expectedParamName = nameof(logger);

                // test target month
                TestNullParameters(services, logger, expectedParamName);
            }
        }
    }
}
