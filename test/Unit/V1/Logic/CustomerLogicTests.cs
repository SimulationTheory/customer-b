using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSE.Customer.Configuration;
using PSE.Customer.V1.Clients.Authentication.Interfaces;
using PSE.Customer.V1.Clients.Authentication.Models.Response;
using PSE.Customer.V1.Logic;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories;
using PSE.Customer.V1.Repositories.Entities;
using PSE.Customer.V1.Repositories.Interfaces;
using PSE.WebAPI.Core.Configuration.Interfaces;
using RestSharp;
using Shouldly;
using System;
using System.Threading.Tasks;

namespace PSE.Customer.Tests.Unit.V1.Logic
{
    [TestClass]
    public class CustomerLogicTests
    {
        private MockRepository mockRepository;

        private Mock<IDistributedCache> mockDistributedCache;
        private Mock<IMemoryCache> mockMemoryCache;
        private Mock<IOptions<AppSettings>> mockOptions;
        private Mock<ILogger<CustomerLogic>> mockLogger;
        private Mock<ICoreOptions> mockCoreOptions;
        private Mock<IBPByContractAccountRepository> mockBPByContractAccountRepository;
        private Mock<ICustomerRepository> mockCustomerRepository;
        private Mock<IAuthenticationApi> mockAuthenticationApi;

        #region Helper Methods
        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockDistributedCache = this.mockRepository.Create<IDistributedCache>();
            this.mockMemoryCache = this.mockRepository.Create<IMemoryCache>();
            this.mockOptions = this.mockRepository.Create<IOptions<AppSettings>>();
            this.mockLogger = this.mockRepository.Create<ILogger<CustomerLogic>>();
            this.mockCoreOptions = this.mockRepository.Create<ICoreOptions>();
            this.mockBPByContractAccountRepository = this.mockRepository.Create<IBPByContractAccountRepository>();
            this.mockCustomerRepository = this.mockRepository.Create<ICustomerRepository>();
            this.mockAuthenticationApi = this.mockRepository.Create<IAuthenticationApi>();
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_DistributedCacheIsNull_ExceptionThrown()
        {
            // Arrange
            mockDistributedCache = null;
            ICustomerLogic logic = null;

            // Act
            Action action = () => logic = CreateCustomerLogic();

            // Assert
            logic.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("redisCache");
        }

        [TestMethod]
        public void Constructor_MemoryCacheIsNull_ExceptionThrown()
        {
            // Arrange
            mockMemoryCache = null;
            ICustomerLogic logic = null;

            // Act
            Action action = () => logic = CreateCustomerLogic();

            // Assert
            logic.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("localCache");
        }

        [TestMethod]
        public void Constructor_OptionsIsNull_ExceptionThrown()
        {
            // Arrange
            mockOptions = null;
            ICustomerLogic logic = null;

            // Act
            Action action = () => logic = CreateCustomerLogic();

            // Assert
            logic.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("appSettings");
        }

        [TestMethod]
        public void Constructor_LoggerIsNull_ExceptionThrown()
        {
            // Arrange
            mockLogger = null;
            ICustomerLogic logic = null;

            // Act
            Action action = () => logic = CreateCustomerLogic();

            // Assert
            logic.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("logger");
        }

        [TestMethod]
        public void Constructor_CoreOptionsIsNull_ExceptionThrown()
        {
            // Arrange
            mockCoreOptions = null;
            ICustomerLogic logic = null;

            // Act
            Action action = () => logic = CreateCustomerLogic();

            // Assert
            logic.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("options");
        }

        [TestMethod]
        public void Constructor_BPByContractAccountRepositoryIsNull_ExceptionThrown()
        {
            // Arrange
            mockBPByContractAccountRepository = null;
            ICustomerLogic logic = null;

            // Act
            Action action = () => logic = CreateCustomerLogic();

            // Assert
            logic.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("bpByContractAccountRepository");
        }

        [TestMethod]
        public void Constructor_CustomerRepositoryIsNull_ExceptionThrown()
        {
            // Arrange
            mockCustomerRepository = null;
            ICustomerLogic logic = null;

            // Act
            Action action = () => logic = CreateCustomerLogic();

            // Assert
            logic.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("customerRepository");
        }

        [TestMethod]
        public void Constructor_AuthenticationApiIsNull_ExceptionThrown()
        {
            // Arrange
            mockAuthenticationApi = null;
            ICustomerLogic logic = null;

            // Act
            Action action = () => logic = CreateCustomerLogic();

            // Assert
            logic.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("authenticationApi");
        }

        #endregion

        #region LookupCustomer Tests

        [TestMethod]
        public async Task LookupCustomer_Returns_LookupCustomerResponse_Given_Valid_Request()
        {
            const string testFullName = "JON SMITH";
            const long bp_Id = 123456789;
            const long acctId = 123456789012;

            // Arrange
            IRestResponse<AccountExistsResponse> restResponse = new RestResponse<AccountExistsResponse>
            {
                Data = new AccountExistsResponse
                {
                    Exists = true,
                }
            };

            mockBPByContractAccountRepository.Setup(bbcar => bbcar.GetBpByContractAccountId(It.IsAny<long>()))
                .Returns((long caId) =>
                {
                    var bbcarEntity = new BPByContractAccountEntity
                    {
                        BusinessPartner_Id = bp_Id,
                    };

                    return Task.FromResult(bbcarEntity);
                });

            mockCustomerRepository.Setup(cr => cr.GetCustomerByBusinessPartnerId(It.IsAny<long>()))
                .Returns((long bpId) =>
                {
                    var customerEntity = new CustomerEntity
                    {
                        FullName = testFullName,
                    };

                    return Task.FromResult(customerEntity);
                });

            mockAuthenticationApi.Setup(cr => cr.GetAccountExists(It.IsAny<long>()))
                .Returns(Task.FromResult(restResponse));

            var lookupCustomerRequest = new LookupCustomerRequest
            {
                ContractAccountNumber = acctId,
                NameOnBill = testFullName,
            };

            // Act
            CustomerLogic customerLogic = this.CreateCustomerLogic();

            var actual = await customerLogic.LookupCustomer(lookupCustomerRequest);


            // Assert
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType<LookupCustomerModel>();
            actual.BPId.ShouldBe(bp_Id);
            actual.HasWebAccount.ShouldBeTrue();
        }

        [TestMethod]
        public async Task LookupCustomer_Returns_Null_On_Bad_AcccountId()
        {
            const string testFullName = "JON SMITH";
            const long acctId = 123456789012;

            // Arrange
            IRestResponse<AccountExistsResponse> restResponse = new RestResponse<AccountExistsResponse>
            {
                Data = new AccountExistsResponse
                {
                    Exists = true,
                }
            };

            mockBPByContractAccountRepository
                .Setup(bbcar => bbcar.GetBpByContractAccountId(It.IsAny<long>()))
                .Returns(Task.FromResult<BPByContractAccountEntity>(null));

            mockCustomerRepository.Setup(cr => cr.GetCustomerByBusinessPartnerId(It.IsAny<long>()))
                .Returns((long bpId) =>
                {
                    var customerEntity = new CustomerEntity
                    {
                        FullName = testFullName,
                    };

                    return Task.FromResult(customerEntity);
                });

            mockAuthenticationApi.Setup(cr => cr.GetAccountExists(It.IsAny<long>()))
                .Returns(Task.FromResult(restResponse));

            var lookupCustomerRequest = new LookupCustomerRequest
            {
                ContractAccountNumber = acctId,
                NameOnBill = testFullName,
            };

            // Act
            CustomerLogic customerLogic = this.CreateCustomerLogic();

            var actual = await customerLogic.LookupCustomer(lookupCustomerRequest);

            // Assert
            actual.ShouldBeNull();
        }

        [TestMethod]
        public async Task LookupCustomer_Returns_Null_On_Bad_Name()
        {
            const string testFullName = "JON SMITH";
            const long bp_Id = 123456789;
            const long acctId = 123456789012;

            // Arrange
            IRestResponse<AccountExistsResponse> restResponse = new RestResponse<AccountExistsResponse>
            {
                Data = new AccountExistsResponse
                {
                    Exists = true,
                }
            };

            mockBPByContractAccountRepository.Setup(bbcar => bbcar.GetBpByContractAccountId(It.IsAny<long>()))
                .Returns((long caId) =>
                {
                    var bbcarEntity = new BPByContractAccountEntity
                    {
                        BusinessPartner_Id = bp_Id,
                    };

                    return Task.FromResult(bbcarEntity);
                });

            mockCustomerRepository.Setup(cr => cr.GetCustomerByBusinessPartnerId(It.IsAny<long>()))
                .Returns((long bpId) =>
                {
                    var customerEntity = new CustomerEntity
                    {
                        FullName = testFullName,
                    };

                    return Task.FromResult(customerEntity);
                });

            mockAuthenticationApi.Setup(cr => cr.GetAccountExists(It.IsAny<long>()))
                .Returns(Task.FromResult(restResponse));

            var lookupCustomerRequest = new LookupCustomerRequest
            {
                ContractAccountNumber = acctId,
                NameOnBill = "A BAD TEST NAME",
            };

            // Act
            CustomerLogic customerLogic = this.CreateCustomerLogic();

            var actual = await customerLogic.LookupCustomer(lookupCustomerRequest);


            // Assert
            actual.ShouldBeNull();
        }

        #endregion

        #region Private Methods
        private CustomerLogic CreateCustomerLogic()
        {
            return new CustomerLogic(
                this.mockDistributedCache?.Object,
                this.mockMemoryCache?.Object,
                this.mockOptions?.Object,
                this.mockLogger?.Object,
                this.mockCoreOptions?.Object,
                this.mockBPByContractAccountRepository?.Object,
                this.mockCustomerRepository?.Object,
                this.mockAuthenticationApi?.Object);
        }
        #endregion
    }
}
