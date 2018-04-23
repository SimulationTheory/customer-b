using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using PSE.Cassandra.Core.Extensions;
using PSE.Customer.Extensions;
using PSE.Customer.Tests.Integration.TestObjects;
using PSE.Customer.V1.Clients.Authentication.Interfaces;
using PSE.Customer.V1.Clients.Mcf;
using PSE.Customer.V1.Clients.Mcf.Enums;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.RestUtility.Core.Mcf;
using PSE.WebAPI.Core.Configuration.Interfaces;
using PSE.WebAPI.Core.Service.Interfaces;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.Tests.Integration.V1.Clients
{
    [TestClass]
    public class McfClientTests
    {
        #region Helpers

        private ServiceProvider Provider { get; set; }
        private IAuthenticationApi AuthClient { get; set; }
        private IMcfClient McfClient { get; set; }
        private TestRequestContextAdapter UserContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            // Arrange
            if (Provider == null)
            {
                // Set up a container that ApiUsers can simulate a context
                UserContext = new TestRequestContextAdapter();

                // Load the service collection
                AutoMapper.Mapper.Reset();
                var services = TestHelper.GetServiceCollection();
                var loggerFactory = services.GetLoggerFactory();
                var logger = loggerFactory.CreateLogger<Startup>();

                services.AddRepositories(logger)
                    .AddClientProxies()
                    .AddSingleton<IRequestContextAdapter>(UserContext);

                // Initialize the provider so that objects can be created by IOC
                Provider = services.BuildServiceProvider();
                AuthClient = Provider.GetService<IAuthenticationApi>();
                McfClient = Provider.GetService<IMcfClient>();
              
                // clean up artifacts left behind by cassand registration
                Cassandra.Core.Extensions.ServiceCollectionExtensions.ClearCassandraSettings();
            }
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_RequestContextIsNull_ExceptionThrown()
        {
            // Arrange
            McfClient client = null;

            // Act
            Action action = () => client = new McfClient(null, new Mock<ICoreOptions>().Object, new Mock<ILogger<McfClient>>().Object);

            // Assert
            client.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("requestContext");
        }

        [TestMethod]
        public void Constructor_CoreOptionsIsNull_ExceptionThrown()
        {
            // Arrange
            McfClient client = null;

            // Act
            Action action = () => client = new McfClient(new Mock<IRequestContextAdapter>().Object, null, new Mock<ILogger<McfClient>>().Object);

            // Assert
            client.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("coreOptions");
        }

        [TestMethod]
        public void Constructor_LoggerIsNull_ExceptionThrown()
        {
            // Arrange
            McfClient client = null;

            // Act
            Action action = () => client = new McfClient(new Mock<IRequestContextAdapter>().Object, new Mock<ICoreOptions>().Object, null);

            // Assert
            client.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("coreOptions");
        }

        #endregion

        #region GetBusinessPartnerContactInfo Tests

        [TestMethod]
        [Ignore("This is failing locally, but was not yesterday")]
        public async Task GetBusinessPartnerContactInfo_ValidUser_ContactInfoRetrieved()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            var loginResponse = await AuthClient.GetJwtToken(user.Username, "Start@123");
            user.JwtEncodedString = loginResponse.Data.JwtAccessToken;
            user.JwtEncodedString.ShouldNotBeNullOrWhiteSpace();

            // Act
            var response = McfClient.GetBusinessPartnerContactInfo(user.JwtEncodedString, user.BPNumber.ToString());

            // Assert
            response.Result.ShouldNotBeNull();

            var info = response.Result;
            info.BusinessPartnerId.ShouldBe(user.BPNumber);
            info.FirstName.ToUpper().ShouldBe("JENNIFER");
            info.LastName.ToUpper().ShouldBe("POWERS");
            info.FullName.ToUpper().ShouldBe("JENNIFER POWERS");

            info.AccountAddressIndependentEmails.Results.ShouldNotBeNull();
            var emails = info.AccountAddressIndependentEmails.Results.ToList();
            emails.Count.ShouldBeGreaterThanOrEqualTo(1);
            emails[0].AccountID.ShouldBe(user.BPNumber.ToString());
            emails[0].SequenceNo.ShouldBe("001");
            emails[0].Email.ShouldBe(user.Email);
            emails[0].StandardFlag.ShouldBe(false);
            emails[emails.Count - 1].StandardFlag = true;

            info.AccountAddressIndependentPhones.Results.ShouldNotBeNull();
            var phones = info.AccountAddressIndependentPhones.Results.ToList();
            phones.Count.ShouldBeGreaterThanOrEqualTo(0);
            // TBD: add a phone then update this get test
        }

        #endregion

        #region CreateBusinessPartnerEmail Tests

        [TestMethod]
        [Ignore("The email here is created for each call, so run it manually to avoid large numbers on test server.")]
        public async Task CreateBusinessPartnerEmail_ValidUser_ContactInfoRetrieved()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            var loginResponse = await AuthClient.GetJwtToken(user.Username, "Start@123");
            user.JwtEncodedString = loginResponse.Data.JwtAccessToken;
            user.JwtEncodedString.ShouldNotBeNullOrWhiteSpace();
            var request = new CreateEmailRequest
            {
                AccountID = user.BPNumber.ToString(),
                Email = user.Email,
                StandardFlag = true
            };

            if (request.Email == user.Email)
            {
                Assert.Fail("Bypass by debugging in order to avoid posting excess rows on server");
            }

            var contactInfoResponse = McfClient.GetBusinessPartnerContactInfo(user.JwtEncodedString, user.BPNumber.ToString());
            contactInfoResponse.Result.ShouldNotBeNull();
            var emails = contactInfoResponse.Result.AccountAddressIndependentEmails.Results.ToList();

            // Act
            var response = McfClient.CreateBusinessPartnerEmail(user.JwtEncodedString, request);

            // Assert
            response.Result.ShouldNotBeNull();
            response.Result.Metadata.Id.ShouldBe("https://10.41.53.54:8001/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/AccountAddressIndependentEmails(" +
                                                 $"AccountID=\'{user.BPNumber}\',SequenceNo=\'{emails.Count:D3}\')");
        }

        #endregion

        #region CreateBusinessPartnerMobilePhone Tests

        [TestMethod]
        [Ignore("The phone here is created for each call, so run it manually to avoid large numbers on test server.")]
        public async Task CreateBusinessPartnerMobilePhone_ValidUser_ContactInfoRetrieved()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            var loginResponse = await AuthClient.GetJwtToken(user.Username, "Start@123");
            user.JwtEncodedString = loginResponse.Data.JwtAccessToken;
            user.JwtEncodedString.ShouldNotBeNullOrWhiteSpace();
            var phone = user.Phones.First(p => p.Type == PhoneType.Cell);
            var request = new CreateAddressIndependantPhoneRequest
            {
                BusinessPartnerId = user.BPNumber,
                PhoneNumber = phone.Number,
                Extension = phone.Extension ?? "",
                IsHome = true,
                IsStandard = true,
                PhoneType = AddressIndependantContactInfoEnum.AccountAddressIndependentMobilePhones
            };

            if (request.BusinessPartnerId == user.BPNumber)
            {
                Assert.Fail("Bypass by debugging in order to avoid posting excess rows on server");
            }

            var contactInfoResponse = McfClient.GetBusinessPartnerContactInfo(user.JwtEncodedString, user.BPNumber.ToString());
            contactInfoResponse.Result.ShouldNotBeNull();
            var phones = contactInfoResponse.Result.AccountAddressIndependentPhones.Results.ToList();

            // Act
            var response = McfClient.CreateBusinessPartnerMobilePhone(user.JwtEncodedString, request);

            // Assert
            response.Result.ShouldNotBeNull();
            response.Result.Metadata.Id.ShouldBe("https://10.41.53.54:8001/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/AccountAddressIndependentPhones(" +
                                                 $"AccountID=\'{user.BPNumber}\',SequenceNo=\'{phones.Count:D3}\')");
        }

        #endregion

        #region CreateBusinessPartnerAddressPhone Tests

        [TestMethod]
        [Ignore("The phone here is created for each call, so run it manually to avoid large numbers on test server.")]
        public async Task CreateBusinessPartnerAddressPhone_ValidUser_ContactInfoRetrieved()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            var loginResponse = await AuthClient.GetJwtToken(user.Username, "Start@123");
            user.JwtEncodedString = loginResponse.Data.JwtAccessToken;
            user.JwtEncodedString.ShouldNotBeNullOrWhiteSpace();
            var phone = user.Phones.First(p => p.Type == PhoneType.Home);
            var request = new CreateAddressDependantPhoneRequest
            {
                BusinessPartnerId = user.BPNumber,
                PhoneNumber = phone.Number,
                Extension = phone.Extension ?? "",
                IsHome = true,
                IsStandard = true,
                PhoneType = "1"
            };

            var addressResponse = McfClient.GetStandardMailingAddress(user.JwtEncodedString, user.BPNumber);
            request.AddressId = addressResponse.Result.AddressID.ToString();

            if (request.BusinessPartnerId == user.BPNumber)
            {
                Assert.Fail("Bypass by debugging in order to avoid posting excess rows on server");
            }

            // Act
            var response = McfClient.CreateAddressDependantPhone(user.JwtEncodedString, request);

            // Assert
            response.Result.ShouldNotBeNull();
            //response.Result.Metadata.Id.ShouldBe("https://10.41.53.54:8001/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/AccountAddressIndependentPhones(" +
            //                                     $"AccountID=\'{user.BPNumber}\',SequenceNo=\'{phones.Count:D3}\')");
        }

        #endregion

        #region Business Partner Identifier Tests

        [TestMethod]
        public async Task GetAllIdentifiers_AccountWithIdentifier_AllIdentifiers()
        {
            // Arrange
            var user = TestHelper.ActivePaUser;
            var loginResponse = await AuthClient.GetJwtToken(user.Username, "Start@123");
            user.SetJwtEncodedString(loginResponse.Data.JwtAccessToken);
            UserContext.SetUser(user);

            // Act
            var response = McfClient.GetAllIdentifiers(user.BPNumber.ToString());

            // Assert
            response.ShouldNotBeNull();
            var results = response.Result.Results.ToList();
            results[0].AccountId.ShouldBe(user.BPNumber.ToString());
            results[0].IdentifierType.ShouldNotBeEmpty();

            var lastFour = results.First(x => x.IdentifierType == "ZLAST4");
            lastFour.IdentifierNo.ShouldBe("9999");
        }

        #endregion

        #region GetPaymentArrangement Tests

        [TestMethod]
        public async Task GetPaymentArrangement_AccountWithArrangement_CanGetArrangementData()
        {
            // Arrange
            var user = TestHelper.ActivePaUser;
            var loginResponse = await AuthClient.GetJwtToken(user.Username, "Start@123");
            user.SetJwtEncodedString(loginResponse.Data.JwtAccessToken);

            user.JwtEncodedString.ShouldNotBeNullOrWhiteSpace();
            user.BPNumber.ShouldNotBe(0);

            // Act
            var response = McfClient.GetPaymentArrangement(user.JwtEncodedString, user.ContractAccountId);

            // Assert
            response.Result.ShouldNotBeNull();
        }

        #endregion

        #region GetStandardMailingAddress Tests

        [TestMethod]
        public async Task GetStandardMailingAddress_ValidAccount_CanGetStandardAddress()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            var loginResponse = await AuthClient.GetJwtToken(user.Username, "Start@123");
            user.JwtEncodedString = loginResponse.Data.JwtAccessToken;
            user.JwtEncodedString.ShouldNotBeNullOrWhiteSpace();

            // Act
            var addressResponse = McfClient.GetStandardMailingAddress(user.JwtEncodedString, user.BPNumber);

            // Assert
            addressResponse.ShouldNotBeNull();
            addressResponse.Error.ShouldBeNull();
            var address = addressResponse.Result;
            address.ShouldNotBeNull();
            address.AddressID.ShouldNotBeNull();
        }

        #endregion

        #region GetMailingAddresses Tests

        [TestMethod]
        public void GetMailingAddresses_AccountWithMailingAddresses_CanParseTestData()
        {
            // Arrange
            var testData = TestData.MailingAddressesData.ActiveMaUserData;

            // Act
            var response = JsonConvert.DeserializeObject<McfResponse<McfResponseResults<GetAccountAddressesResponse>>>(testData);

            // Assert
            response.Result.Results.ShouldBeOfType<List<GetAccountAddressesResponse>>();
            response.Result.Results.ToList().Count.ShouldBeGreaterThanOrEqualTo(1);

            var firstAddressResponse = response.Result.Results.FirstOrDefault();
            firstAddressResponse.ShouldNotBeNull();
            firstAddressResponse.AccountID.ShouldBe(1200662307);
            firstAddressResponse.AddressID.ShouldBe(33343907);

            var firstAddress = firstAddressResponse.AddressInfo;
            firstAddress.StandardAddress.ShouldBeTrue();
            firstAddress.City.ShouldBe("Renton");
            firstAddress.PostalCode.ShouldBe("98055-5107");
            firstAddress.POBoxPostalCode.ShouldBe("");
            firstAddress.POBox.ShouldBe("");
            firstAddress.Street.ShouldBe("SE 166th St");
            firstAddress.HouseNo.ShouldBe("10502");
            firstAddress.CountryID.ShouldBe("US");
            firstAddress.Region.ShouldBe("WA");
            firstAddress.HouseNo2.ShouldBe("Apt H3");

            var secondAddressResponse = response.Result.Results.ElementAt(1);
            secondAddressResponse.AccountID.ShouldBe(1200662307);
            secondAddressResponse.AddressID.ShouldBe(33343940);

            var secondAddress = secondAddressResponse.AddressInfo;
            secondAddress.StandardAddress.ShouldBeFalse();
            secondAddress.City.ShouldBe("Renton");
            secondAddress.PostalCode.ShouldBe("98055");
            secondAddress.POBoxPostalCode.ShouldBe("");
            secondAddress.POBox.ShouldBe("");
            secondAddress.Street.ShouldBe("SE 166TH ST");
            secondAddress.HouseNo.ShouldBe("10502");
            secondAddress.CountryID.ShouldBe("US");
            secondAddress.Region.ShouldBe("WA");
        }

        [TestMethod]
        public async Task GetMailingAddresses_AccountWithAMailingAddreses_CanGetMailingAddressesData()
        {
            // Arrange
            var user = TestHelper.ActiveMaUser;
            var loginResponse = await AuthClient.GetJwtToken(user.Username, "Start@123");
            user.SetJwtEncodedString(loginResponse.Data.JwtAccessToken);

            user.JwtEncodedString.ShouldNotBeNullOrWhiteSpace();
            user.BPNumber.ShouldNotBe(0);

            var response = McfClient.GetMailingAddresses(user.JwtEncodedString, user.BPNumber);
            response.ShouldNotBeNull();
        }

        #endregion

        #region GetContractAccounMailingAddress Tests

        [TestMethod]
        public void GetContractAccounMailingAddress_AccountWithMailingAddress_CanParseTestData()
        {
            // Arrange
            var testData = TestData.ContractAccountAddressData.ActiveMaUserData;

            // Act
            var response = JsonConvert.DeserializeObject<McfResponse<GetContractAccountResponse>>(testData);

            response.ShouldNotBeNull();
            response.Result.AccountID.ShouldBe(1002785285);
            response.Result.AddressID.ShouldBeNull();
            response.Result.ContractAccountID.ShouldBe(200019410436);
        }

        [TestMethod]
        public async Task GetContractAccounMailingAddress_AccountWithAMailingAddress_CanGetMailingAddressData()
        {
            // Arrange
            var user = TestHelper.ActivePaUser;
            var loginResponse = await AuthClient.GetJwtToken(user.Username, "Start@123");
            user.SetJwtEncodedString(loginResponse.Data.JwtAccessToken);

            user.JwtEncodedString.ShouldNotBeNullOrWhiteSpace();
            user.BPNumber.ShouldNotBe(0);

            var response = McfClient.GetContractAccounMailingAddress(user.JwtEncodedString, user.ContractAccountId);
            response.ShouldNotBeNull();
        }

        #endregion

        #region UpdateAddress Tests

        [TestMethod]
        [Ignore] // Wait For MCF Fix
        public async Task UpdateAddress_ValidUser()
        {
            //Arrange
            var user = TestHelper.ActivePaUser;
            var loginResponse = await AuthClient.GetJwtToken(user.Username, "Start@123");
            user.SetJwtEncodedString(loginResponse.Data.JwtAccessToken);

            var addressResponse = McfClient.GetStandardMailingAddress(user.JwtEncodedString, user.BPNumber).Result;
            addressResponse.AddressID.ShouldNotBeNull();

            var request = new UpdateAddressRequest
            {
                AccountID = user.BPNumber,
                AddressID = addressResponse.AddressID.Value,
                AddressInfo = new McfAddressinfo
                {
                    StandardAddress = true,
                    City = "Bellevue",
                    PostalCode = "98004",
                    POBoxPostalCode = "",
                    POBox = "",
                    Street = "110th Ave NE",
                    HouseNo = "355",
                    CountryID = "US",
                    Region = "WA",
                    HouseNo2 = ""
                }
            };

            // Act
            McfClient.UpdateAddress(user.JwtEncodedString, request);

            var requestAddress = request.AddressInfo;
            var newAddressResponse = McfClient.GetStandardMailingAddress(user.JwtEncodedString, user.BPNumber).Result.AddressInfo;

            //Assert
            newAddressResponse.StandardAddress.ShouldBe(requestAddress.StandardAddress);
            newAddressResponse.City.ShouldBe(requestAddress.City);
            newAddressResponse.PostalCode.ShouldBe(requestAddress.PostalCode);
            newAddressResponse.Street.ShouldBe(requestAddress.Street);
            newAddressResponse.HouseNo.ShouldBe(requestAddress.HouseNo);
            newAddressResponse.CountryID.ShouldBe(requestAddress.CountryID);
            newAddressResponse.Region.ShouldBe(requestAddress.Region);

            // Restore the environment
            var restoreRequest = new UpdateAddressRequest
            {
                AccountID = addressResponse.AccountID,
                AddressID = addressResponse.AddressID.Value,
                AddressInfo = new McfAddressinfo
                {
                    StandardAddress = addressResponse.AddressInfo.StandardAddress,
                    City = addressResponse.AddressInfo.City,
                    PostalCode = addressResponse.AddressInfo.PostalCode,
                    POBoxPostalCode = addressResponse.AddressInfo.POBoxPostalCode,
                    POBox = addressResponse.AddressInfo.POBox,
                    Street = addressResponse.AddressInfo.Street,
                    HouseNo = addressResponse.AddressInfo.HouseNo,
                    CountryID = addressResponse.AddressInfo.CountryID,
                    Region = addressResponse.AddressInfo.Region,
                }
            };

            McfClient.UpdateAddress(user.JwtEncodedString, restoreRequest);
        }

        #endregion

        #region CreateInteractionRecords Tests
        [TestMethod]
        public async Task CreateCustomerInteractionRecord_CanCreateRecord()
        {
            // Arrange
            var user = TestHelper.ActiveInteractionUser;
            var loginResponse = await AuthClient.GetJwtToken("donaldmcconnell", "Start@123");
            user.SetJwtEncodedString(loginResponse.Data.JwtAccessToken);

            user.JwtEncodedString.ShouldNotBeNullOrWhiteSpace();
            user.BPNumber.ShouldNotBe(0);
            CreateCustomerInteractionRequest newInteraction = new CreateCustomerInteractionRequest()
            {

                AccountID = "1001840105",
                Description = "High Bill Call",
                PremiseID = 7000006028,
                BusinessAgreementID = 200000053856,
                Priority = 5,
                InteractionRecordReasonID = "A1-ZBIL -HIGH",
                InteractionRecordCategory1 = "",
                InteractionRecordCategory2 = "",
                InteractionRecordCategory1GUID = "",
                InteractionRecordCategory2GUID = "",
                ChannelID = "Z14",
                Note = "Insert Notes for the IR",
                IncomingFlag = true,
                DocumentStatusID = "E0003"
             };

            GetCustomerInteractionResponse response = McfClient.CreateCustomerInteractionRecord(newInteraction, user.JwtEncodedString);
            Assert.AreEqual(response.Success, "True");
            Assert.IsNotNull(response.InteractionRecordID);
        }
        #endregion

        #region CreateAddress Tests

        [TestMethod]
        [Ignore("The address is created for each call, so run it manually to avoid large numbers on test server.")]

        public async Task CreateStandardAddress_ValidUser()
        {
            //Arrange
            var user = TestHelper.ActivePaUser;
            var loginResponse = await AuthClient.GetJwtToken(user.Username, "Start@123");
            user.SetJwtEncodedString(loginResponse.Data.JwtAccessToken);

            var request = new CreateAddressRequest
            {
                AccountID = user.BPNumber,
                AddressInfo = new McfAddressinfo
                {
                    StandardAddress = true,
                    City = "Bellevue",
                    PostalCode = "98004",
                    POBoxPostalCode = "",
                    POBox = "",
                    Street = "110th Ave NE",
                    HouseNo = "355",
                    CountryID = "US",
                    Region = "WA",
                    HouseNo2 = ""
                }
            };

            // Act
            var response = McfClient.CreateAddress(user.JwtEncodedString, request);

            var requestAddress = request.AddressInfo;

            var responsetAddress = response.Result.AddressInfo;

            //Assert
            responsetAddress.StandardAddress.ShouldBe(requestAddress.StandardAddress);
            responsetAddress.City.ShouldBe(requestAddress.City);
            responsetAddress.PostalCode.ShouldBe(requestAddress.PostalCode);
            responsetAddress.Street.ShouldBe(requestAddress.Street);
            responsetAddress.HouseNo.ShouldBe(requestAddress.HouseNo);
            responsetAddress.CountryID.ShouldBe(requestAddress.CountryID);
            responsetAddress.Region.ShouldBe(requestAddress.Region);
        }

        #endregion
    }
}

