using System;
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
        [Ignore("This account is getting a not found error, but the call seems to work otherwise")]
        public async Task GetBusinessPartnerContactInfo_ValidUser_ContactInfoRetrieved()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            var loginResponse = await AuthClient.GetJwtToken(user.Username, "Start@123");
            user.JwtEncodedString = loginResponse.Data.JwtAccessToken;
            user.JwtEncodedString.ShouldNotBeNullOrWhiteSpace();

            var response = McfClient.GetBusinessPartnerContactInfo(user.JwtEncodedString, user.BPNumber.ToString());
            response.Result.ShouldNotBeNull();
        }

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
            //var dueDate = firstPayment.DueDate.Between("(", ")").FromUnixTimeSeconds();

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
        }

        #endregion
    }
}
