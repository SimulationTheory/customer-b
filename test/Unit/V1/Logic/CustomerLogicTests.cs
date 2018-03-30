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
using System.Collections.Generic;
using System.Threading.Tasks;
using Cassandra;
using PSE.Customer.Tests.Unit.TestObjects;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.RestUtility.Core.Mcf;

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
        private Mock<IMcfClient> mockMcfClient;

        #region Helper Methods

        private CustomerLogic CreateCustomerLogic()
        {
            return new CustomerLogic(
                mockDistributedCache?.Object,
                mockMemoryCache?.Object,
                mockOptions?.Object,
                mockLogger?.Object,
                mockCoreOptions?.Object,
                mockBPByContractAccountRepository?.Object,
                mockCustomerRepository?.Object,
                mockAuthenticationApi?.Object,
                mockMcfClient?.Object);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Loose);

            mockDistributedCache = mockRepository.Create<IDistributedCache>();
            mockMemoryCache = mockRepository.Create<IMemoryCache>();
            mockOptions = mockRepository.Create<IOptions<AppSettings>>();
            mockLogger = mockRepository.Create<ILogger<CustomerLogic>>();
            mockCoreOptions = mockRepository.Create<ICoreOptions>();
            mockBPByContractAccountRepository = mockRepository.Create<IBPByContractAccountRepository>();
            mockCustomerRepository = mockRepository.Create<ICustomerRepository>();
            mockAuthenticationApi = mockRepository.Create<IAuthenticationApi>();
            mockMcfClient = mockRepository.Create<IMcfClient>();
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
            CustomerLogic customerLogic = CreateCustomerLogic();

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

            mockAuthenticationApi.Setup(cr => cr.GetAccountExists(It.IsAny<long>()))
                .Returns(Task.FromResult(restResponse));

            var lookupCustomerRequest = new LookupCustomerRequest
            {
                ContractAccountNumber = acctId,
                NameOnBill = testFullName,
            };

            // Act
            CustomerLogic customerLogic = CreateCustomerLogic();

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
            CustomerLogic customerLogic = CreateCustomerLogic();

            var actual = await customerLogic.LookupCustomer(lookupCustomerRequest);


            // Assert
            actual.ShouldBeNull();
        }

        #endregion

        #region GetCustomerProfileAsync Tests

        [TestMethod]
        public async Task GetCustomerProfileAsync_ValidBpNumber_ReturnsCustomerProfile()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            var logic = CreateCustomerLogic();

            mockCustomerRepository.Setup(cr => cr.GetCustomerAsync(It.IsAny<long>()))
                .Returns((long bpId) =>
                {
                    var customerEntity = new CustomerEntity
                    {
                        BusinessPartnerId = user.BpNumber,
                        FullName = "JENNIFER L POWERS",
                        FirstName = "JENNIFER",
                        LastName = "POWERS"
                    };
                    return Task.FromResult(customerEntity);
                });

            mockCustomerRepository.Setup(cr => cr.GetCustomerContactAsync(It.IsAny<long>()))
                .Returns((long bpId) =>
                {
                    var customerContactEntity = new CustomerContactEntity
                    {
                        Email = user.Email,
                        MailingAddress = user.Address,
                        Phones = new Dictionary<string, PhoneDefinedType>
                        {
                            {
                                "cell", new PhoneDefinedType
                                {
                                    Number = user.Phones[0].Number,
                                    Extension = user.Phones[0].Extension
                                }
                            },
                            {
                                "work", new PhoneDefinedType
                                {
                                    Number = user.Phones[1].Number,
                                    Extension = user.Phones[1].Extension
                                }
                            }
                        }
                    };
                    return Task.FromResult(customerContactEntity);
                });

            // Act
            var response = await logic.GetCustomerProfileAsync(user.BpNumber);

            // Assert
            response.ShouldNotBeNull();
            response.ShouldBeOfType<CustomerProfileModel>();
            response.MailingAddress.AddressLine1.ShouldBe(user.Address.AddressLine1);
        }

        #endregion

        #region PutMailingAddressAsync Tests

        [TestMethod]
        public async Task PutMailingAddressAsync_ValidAddress_MailingAddressUpdated()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            var logic = CreateCustomerLogic();
            mockCustomerRepository.Setup(x => x.UpdateCustomerMailingAddress(It.IsAny<AddressDefinedType>(), It.IsAny<long>()))
                .Returns(Task.FromResult(new RowSet()));

            // Act
            await logic.PutMailingAddressAsync(user.Address, user.BpNumber);
        }

        #endregion

        #region PutEmailAddressAsync Tests

        [TestMethod]
        public async Task PutEmailAddressAsync_ValidAddress_EmailAddressUpdated()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            var logic = CreateCustomerLogic();
            mockCustomerRepository.Setup(x => x.UpdateCustomerEmailAddress(It.IsAny<string>(), It.IsAny<long>()))
                .Returns(Task.FromResult(new RowSet()));

            // Act
            await logic.PutEmailAddressAsync(user.JwtToken, user.Email, user.BpNumber);
        }

        #endregion

        #region PutPhoneNumbersAsync Tests

        [TestMethod]
        public async Task PutPhoneNumbersAsync_CellPhoneAtBpLevel_PhoneNumberUpdated()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            var logic = CreateCustomerLogic();
            mockCustomerRepository.Setup(x => x.UpdateCustomerPhoneNumber(It.IsAny<Phone>(), It.IsAny<long>()))
                .Returns(Task.FromResult(new RowSet()));
            mockMcfClient.Setup(x => x.CreateBusinessPartnerMobilePhone(It.IsAny<string>(), It.IsAny<CreateAddressIndependantPhoneRequest>()))
                .Returns(new McfResponse<GetPhoneResponse>
                {
                    Error = null,
                    Result = new GetPhoneResponse
                    {
                        AccountID = user.ContractAccountId,
                        PhoneNo = user.Phones[0].Number,
                        PhoneType = "2",
                        StandardFlag = true
                    }
                });

            // Act
            await logic.PutPhoneNumberAsync(user.JwtToken, user.Phones[0], user.BpNumber);
        }

        [TestMethod]
        public void PutPhoneNumbersAsync_ThrowsException_ExceptionRelayedToCaller()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            var logic = CreateCustomerLogic();
            mockCustomerRepository.Setup(x => x.UpdateCustomerPhoneNumber(It.IsAny<Phone>(), It.IsAny<long>()))
                .Returns(Task.FromResult(new RowSet()));
            mockMcfClient.Setup(x => x.CreateBusinessPartnerMobilePhone(It.IsAny<string>(), It.IsAny<CreateAddressIndependantPhoneRequest>()))
                .Throws(new ApplicationException());

            // Act
            Func<Task> action = async () => { await logic.PutPhoneNumberAsync(user.JwtToken, user.Phones[0], user.BpNumber); };

            // Assert
            action.ShouldThrow<ApplicationException>();
        }

        #endregion
    }
}
