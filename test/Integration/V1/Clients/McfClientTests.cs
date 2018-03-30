using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using PSE.Customer.Configuration;
using PSE.Customer.Extensions;
using PSE.Customer.Tests.Integration.TestObjects;
using PSE.Customer.V1.Clients.Authentication.Interfaces;
using PSE.Customer.V1.Clients.Extensions;
using PSE.Customer.V1.Clients.Mcf;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.Customer.V1.Response;
using PSE.RestUtility.Core.Mcf;
using PSE.Test.Core;
using PSE.WebAPI.Core.Configuration.Interfaces;
using PSE.WebAPI.Core.Startup;
using Shouldly;

namespace PSE.Customer.Tests.Integration.V1.Clients
{
#if DEBUG
    // Exclude in release mode since load balancer machine is not available from build server
    [TestClass]
#endif
    public class McfClientTests
    {
        #region Helpers

        private ServiceProvider Provider { get; set; }
        private IAuthenticationApi AuthClient { get; set; }
        private IMcfClient McfClient { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            // Arrange
            if (Provider == null)
            {
                // Load the service collection
                var logger = CoreHelper.GetLogger<Startup>();
                AutoMapper.Mapper.Reset();
                var services = TestHelper.GetServiceCollection();
                services.ConfigurePSEWebAPI(ServiceConfiguration.AppName);
                services.AddRepositories(logger)
                    .AddClientProxies();

                // Initialize the provider so that objects can be created by IOC
                Provider = services.BuildServiceProvider();
                AuthClient = Provider.GetService<IAuthenticationApi>();
                McfClient = Provider.GetService<IMcfClient>();
            }
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_CoreOptionsIsNull_ExceptionThrown()
        {
            // Arrange
            McfClient client = null;

            // Act
            Action action = () => client = new McfClient(null, new Mock<ILogger<McfClient>>().Object);

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
            Action action = () => client = new McfClient(new Mock<ICoreOptions>().Object, null);

            // Assert
            client.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("coreOptions");
        }

        #endregion

        #region GetAccountAddress Tests

        [TestMethod]
        public async Task GetBusinessPartnerContactInfo_ValidUser_ContactInfoRetrieved()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            var loginResponse = await AuthClient.GetJwtToken(user.Username, "Start@123");
            user.JwtEncodedString = loginResponse.Data.JwtAccessToken;
            user.JwtEncodedString.ShouldNotBeNullOrWhiteSpace();

            var response = McfClient.GetBusinessPartnerContactInfo(user.JwtEncodedString, user.BPNumber.ToString());
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

        #region GetPaymentArrangement Tess

        [TestMethod]
        public void GetPaymentArrangement_AccountWithArrangement_CanParseTestData()
        {
            // Arrange
            var testData = TestData.PaymentArrangementData.ActivePaUserFakeGetData;

            // Act
            var response = JsonConvert.DeserializeObject<McfResponse<PaymentArrangementResponse>>(testData);

            response.Result.ShouldBeOfType<PaymentArrangementResponse>();
            response.Result.Results.Count.ShouldBeGreaterThanOrEqualTo(1);

            var paResult = response.Result.Results[0];
            paResult.ContractAccountID.ShouldBe("200019410436");
            paResult.NoOfPaymentsPermitted.ShouldBe("000");
            paResult.PaStatus.ShouldBe("D");
            paResult.InstallmentPlanNumber.ShouldBe("");
            paResult.InstallmentPlanType.ShouldBe("");
            paResult.PaymentArrangementNav.Results.Count.ShouldBeGreaterThan(0);

            var arrangement = paResult.PaymentArrangementNav.Results[0];
            arrangement.InstallmentPlanNumber.ShouldBe("400000002557");
            arrangement.InstallmentPlanType.ShouldBe("05");
            arrangement.NoOfInstallments.ShouldBe("002");
            arrangement.InstallmentPlansNav.Results.Count.ShouldBeGreaterThan(0);

            // First payment is payed so amount open is zero.  Second payment, unpaid amount is amount due.
            var firstPayment = arrangement.InstallmentPlansNav.Results[0];
            firstPayment.AmountDue.ShouldBe(500m);
            firstPayment.AmountOpen.ShouldBe(0);
            firstPayment.DueDate.ShouldBe("/Date(1518652800000)/");

            firstPayment.DueDate.Between("(", ")").ShouldBe("1518652800000");
            var dueDate = firstPayment.DueDate.Between("(", ")").FromUnixTimeSeconds();
            dueDate.ShouldNotBeNull();
            dueDate.Value.ShouldBe(new DateTime(2018, 2, 15, 0, 0, 0));

            var secondPayment = arrangement.InstallmentPlansNav.Results[1];
            secondPayment.AmountDue.ShouldBe(228.51m);
            secondPayment.AmountOpen.ShouldBe(228.51m);
            secondPayment.DueDate.ShouldBe("/Date(1519862400000)/");
        }

        [TestMethod]
        public async Task GetPaymentArrangement_AccountWithArrangement_CanGetArrangementData()
        {
            // Arrange
            var user = TestHelper.ActivePaUser;
            var loginResponse = await AuthClient.GetJwtToken(user.Username, "Start@123");
            user.SetJwtEncodedString(loginResponse.Data.JwtAccessToken);

            user.JwtEncodedString.ShouldNotBeNullOrWhiteSpace();
            user.BPNumber.ShouldNotBe(0);

            var response = McfClient.GetPaymentArrangement(user.JwtEncodedString, user.ContractAccountId);
            response.Result.ShouldNotBeNull();
        }

        #endregion

        #region CreateBusinessPartnerEmail Tests

        [TestMethod]
        [Ignore("The phone here is created for each call.  May try POST.")]
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

            var response = McfClient.CreateBusinessPartnerEmail(user.JwtEncodedString, request);
            response.Result.ShouldNotBeNull();
            response.Result.Metadata.Id.ShouldBe("https://10.41.53.54:8001/sap/opu/odata/sap/ZERP_UTILITIES_UMC_PSE_SRV/AccountAddressIndependentEmails(AccountID=\'1002647070\',SequenceNo=\'015\')");
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

             response.Result.Results.ShouldBeOfType<List<GetAccountAddressesResponse>>();
             response.Result.Results.ToList().Count.ShouldBeGreaterThanOrEqualTo(1);

            var firstAddressResponse = response.Result.Results.FirstOrDefault();
            firstAddressResponse.AccountID.ShouldBe(1200662307);
            firstAddressResponse.AddressID.ShouldBe(33343907);

            var firstAddress = firstAddressResponse.AddressInfo;
            firstAddress.StandardFlag.ShouldBe("X");
            firstAddress.City.ShouldBe("Renton");
            firstAddress.PostalCode.ShouldBe("98055-5107");
            firstAddress.POBoxPostalCode.ShouldBe("");
            firstAddress.POBox.ShouldBe("");
            firstAddress.Street.ShouldBe("SE 166th St");
            firstAddress.HouseNo.ShouldBe("10502");
            firstAddress.RoomNo.ShouldBe("");
            firstAddress.CountryID.ShouldBe("US");
            firstAddress.CountryName.ShouldBe("USA");
            firstAddress.Region.ShouldBe("WA");


            var secondAddressResponse = response.Result.Results.ElementAt(1);
            secondAddressResponse.AccountID.ShouldBe(1200662307);
            secondAddressResponse.AddressID.ShouldBe(33343940);

            var secondAddress = secondAddressResponse.AddressInfo;
            secondAddress.StandardFlag.ShouldBe("");
            secondAddress.City.ShouldBe("Renton");
            secondAddress.PostalCode.ShouldBe("98055");
            secondAddress.POBoxPostalCode.ShouldBe("");
            secondAddress.POBox.ShouldBe("");
            secondAddress.Street.ShouldBe("SE 166TH ST");
            secondAddress.HouseNo.ShouldBe("10502");
            secondAddress.RoomNo.ShouldBe("");
            secondAddress.CountryID.ShouldBe("US");
            secondAddress.CountryName.ShouldBe("USA");
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

            var response = McfClient.GetMailingAddresses(user.JwtEncodedString, user.ContractAccountId);
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
            response.Result.AddressID.ShouldBeEmpty();
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
    }
}
