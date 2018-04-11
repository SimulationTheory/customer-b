using System.Threading.Tasks;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSE.Customer.Tests.Integration.TestObjects;
using PSE.Customer.V1.Clients.Authentication;
using PSE.Customer.V1.Clients.Authentication.Interfaces;
using PSE.Customer.V1.Clients.Authentication.Models.Response;
using PSE.Customer.V1.Clients.ClientProxy.Interfaces;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Configuration;
using PSE.WebAPI.Core.Configuration.Interfaces;
using RestSharp;
using Shouldly;

namespace PSE.Customer.Tests.Integration.V1.Clients
{
    [TestClass]
    public class AuthenticationApiTests
    {
        #region Helper Methods

        private Mock<ICoreOptions> CoreOptionsMock { get; set; }

        private IAuthenticationApi GetApi(IApiUser apiUser = null)
        {
            var api = new AuthenticationApi(CoreOptionsMock?.Object);
            return api;
        }

        /// <summary>
        /// Configures the primary object under test with default mocks and setup to allow other tests to be run easily.
        /// This does NOT create the object under test to allow mock setup to be adjusted in the tests Assert section.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            // The WebConfiguration does not have an interface, so a concrete type must be used.
            CoreOptionsMock = new Mock<ICoreOptions>();
            CoreOptionsMock.SetupGet(x => x.Configuration).Returns(
                new WebConfiguration { LoadBalancerUrl = "http://internal-ci-dev-alb-478417170.us-west-2.elb.amazonaws.com" });
        }

        #endregion

        #region GetAccountExists Tests

        [TestMethod]
        public async Task GetAccountExists_TestUser1_ReturnsTrue()
        {
            // Arrange
            var testUser = TestHelper.PaDev1;
            var client = GetApi();

            // Act
            var response = await client.GetAccountExists(testUser.BPNumber);

            // Assert
            response.Data.ShouldBeOfType(typeof(AccountExistsResponse));
            response.Data.Exists.ShouldBe(true);
        }

        [TestMethod]
        public async Task GetAccountExists_UnknownUsername_ReturnsFalse()
        {
            // Arrange
            var client = GetApi();

            // Act
            var response = await client.GetAccountExists(123456);

            // Assert
            response.ShouldBeOfType(typeof(RestResponse<AccountExistsResponse>));
            response.Data.Exists.ShouldBe(false);
        }

        #endregion

        #region GetUserNameExists Tests

        [TestMethod]
        public async Task GetUserNameExists_TestUser1_ReturnsTrue()
        {
            // Arrange
            var testUser = TestHelper.PaDev1;
            var client = GetApi();

            // Act
            var response = await client.GetUserNameExists(testUser.Username);

            // Assert
            response.Data.ShouldBeOfType(typeof(ExistsResponse));
            response.Data.Exists.ShouldBe(true);
        }

        [TestMethod]
        public async Task GetUserNameExists_UnknownUsername_ReturnsFalse()
        {
            // Arrange
            var client = GetApi();

            // Act
            var response = await client.GetUserNameExists("abraham.lincoln");

            // Assert
            response.ShouldBeOfType(typeof(RestResponse<ExistsResponse>));
            response.Data.Exists.ShouldBe(false);
        }

        #endregion

        #region GetJwtToken Tests

        [TestMethod]
        public async Task GetJwtToken_TestUser1_ReturnsTrue()
        {
            // Arrange
            var testUser = TestHelper.PaDev1;
            var client = GetApi();

            // Act
            var response = await client.GetJwtToken(testUser.Username, "Start@123");

            // Assert
            response.Data.ShouldBeOfType(typeof(SignInResponse));
            response.Data.JwtAccessToken.ShouldNotBeNullOrWhiteSpace();
        }

        [TestMethod]
        [ExpectedException(typeof(XmlException))]
        public async Task GetJwtToken_UnknownUsername_ThrowsException()
        {
            // Arrange
            var client = GetApi();

            // Act
            // Current behavior is that this throws an XmlException when trying to parse the data into a SignInResponse type
            // TODO: Alter ClientProxy approach to handle error return types as well as success return types
            //var response = 
            await client.GetJwtToken("abraham.lincoln", "Start@123");

            // Assert
            //response.ShouldBeOfType(typeof(RestResponse<SignInResponse>));
            //response.Data.Exists.ShouldBe(false);
        }

        #endregion
    }
}
