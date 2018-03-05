using System;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSE.Cassandra.Core.Session.Interfaces;
using PSE.Customer.V1.Repositories.Entities;
using PSE.Customer.V1.Repositories.Views;
using PSE.Test.Core;

// ReSharper disable once CheckNamespace
namespace PSE.Customer.V1.Repositories.Tests.Unit
{
    [TestClass]
    public class ContractAccountRepositoryTests
    {
        [TestClass]
        public class Constructor
        {
            #region Test Helper Methods
            private void TestNullParameters(IEntity<ContractAccountEntity> accountSession, IEntity<ContractAccountByBusinessPartnerView> bpSession, ILogger<ContractAccountRepository> logger, string expectedParamName)
            {
                try
                {
                    // test target constructor
                    var result = new ContractAccountRepository(accountSession, bpSession, logger);

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
            public void ContractAccountRepository_Test()
            {
                // init vars
                var accountSession = new Mock<IEntity<ContractAccountEntity>>().Object;
                var bpSession = new Mock<IEntity<ContractAccountByBusinessPartnerView>>().Object;
                var logger = CoreHelper.GetLogger<ContractAccountRepository>();

                // test target constructor
                var result = new ContractAccountRepository(accountSession, bpSession, logger);
            }

            [TestMethod]
            public void ContractAccountRepository_NullAccountSessionThrows_Test()
            {
                // init vars
                const IEntity<ContractAccountEntity> accountSession = null;
                var bpSession = new Mock<IEntity<ContractAccountByBusinessPartnerView>>().Object;
                var logger = CoreHelper.GetLogger<ContractAccountRepository>();
                const string expectedParamName = nameof(accountSession);

                TestNullParameters(accountSession, bpSession, logger, expectedParamName);
            }

            [TestMethod]
            public void ContractAccountRepository_NullBPSessionThrows_Test()
            {
                // init vars
                var accountSession = new Mock<IEntity<ContractAccountEntity>>().Object;
                IEntity<ContractAccountByBusinessPartnerView> bpSession = null;
                var logger = CoreHelper.GetLogger<ContractAccountRepository>();
                const string expectedParamName = nameof(bpSession);

                TestNullParameters(accountSession, bpSession, logger, expectedParamName);
            }

            [TestMethod]
            public void ContractAccountRepository_NullLoggerThrows_Test()
            {
                // init vars
                var accountSession = new Mock<IEntity<ContractAccountEntity>>().Object;
                var bpSession = new Mock<IEntity<ContractAccountByBusinessPartnerView>>().Object;
                const ILogger<ContractAccountRepository> logger = null;
                const string expectedParamName = nameof(logger);

                TestNullParameters(accountSession, bpSession, logger, expectedParamName);
            }
        }
    }
}