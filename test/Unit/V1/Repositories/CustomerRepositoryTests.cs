using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSE.Cassandra.Core.Session.Interfaces;
using PSE.Customer.Configuration.Keyspaces;
using PSE.Customer.V1.Repositories;
using PSE.Customer.V1.Repositories.Entities;
using PSE.Test.Core;
using System;

namespace PSE.Customer.Tests.Unit.V1.Repositories
{

    [TestClass]
    public class CustomerRepositoryTests
    {
        private MockRepository mockRepository;

        private Mock<ILogger<CustomerRepository>> mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockLogger = mockRepository.Create<ILogger<CustomerRepository>>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestClass]
        public class Constructor
        {
            #region Test Helper Methods
            private void TestNullParameters(
                ISessionFacade<MicroservicesKeyspace> session,
                IEntity<CustomerEntity> customerEntity,
                IEntity<CustomerContactEntity> customerContactEntity,
                ILogger<CustomerRepository> logger,
                string expectedParamName)
            {
                try
                {
                    // test target constructor
                    var result = new CustomerRepository(session, customerEntity, customerContactEntity,  logger);

                    Assert.Fail("The expected ArgumentNullException was not thrown.");
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine(ex);

                    Assert.AreEqual(expectedParamName, ex.ParamName);
                }
            }
            #endregion Test Helper Methods
            [TestMethod]
            public void CustomerRepository_Constructor_Test()
            {
                // init vars
                var session = new Mock<ISessionFacade<MicroservicesKeyspace>>().Object;
                var customerEntity = new Mock<IEntity<CustomerEntity>>().Object;
                var customerContactEntity = new Mock<IEntity<CustomerContactEntity>>().Object;
                var logger = CoreHelper.GetLogger<CustomerRepository>();

                var repo = new CustomerRepository(session, customerEntity, customerContactEntity, logger);
            }

            [TestMethod]
            public void CustomerRepositoryy_Constructor_NullSessionThrows_Test()
            {
                // init vars
                const ISessionFacade<MicroservicesKeyspace> session = null;
                var customerEntity = new Mock<IEntity<CustomerEntity>>().Object;
                var customerContactEntity = new Mock<IEntity<CustomerContactEntity>>().Object;
                var logger = CoreHelper.GetLogger<CustomerRepository>();
                const string expectedParamName = nameof(session);

                TestNullParameters(session, customerEntity, customerContactEntity,  logger, expectedParamName);
            }

            [TestMethod]
            public void AccountSummaryRepository_Constructor_NullCustomerEntityThrows_Test()
            {
                // init vars
                var session = new Mock<ISessionFacade<MicroservicesKeyspace>>().Object;
                const IEntity<CustomerEntity> customer = null;
                var customerContactEntity = new Mock<IEntity<CustomerContactEntity>>().Object;
                var logger = CoreHelper.GetLogger<CustomerRepository>();
                const string expectedParamName = nameof(customer);

                TestNullParameters(session, customer, customerContactEntity,  logger, expectedParamName);
            }

            [TestMethod]
            public void AccountSummaryRepository_Constructor_NullCustomerContactEntityThrows_Test()
            {
                // init vars
                var session = new Mock<ISessionFacade<MicroservicesKeyspace>>().Object;
                var customerEntity = new Mock<IEntity<CustomerEntity>>().Object;
                const IEntity<CustomerContactEntity> customerContact = null;
                var logger = CoreHelper.GetLogger<CustomerRepository>();
                const string expectedParamName = nameof(customerContact);

                TestNullParameters(session, customerEntity, customerContact, logger, expectedParamName);
            }

            [TestMethod]
            public void AccountSummaryRepository_Constructor_NullLoggerThrows_Test()
            {
                // init vars
                var session = new Mock<ISessionFacade<MicroservicesKeyspace>>().Object;
                var customerEntity = new Mock<IEntity<CustomerEntity>>().Object;
                var customerContactEntity = new Mock<IEntity<CustomerContactEntity>>().Object;
                const ILogger<CustomerRepository> logger = null;
                const string expectedParamName = nameof(logger);

                TestNullParameters(session, customerEntity, customerContactEntity, logger, expectedParamName);
            }
        }
    }
}
