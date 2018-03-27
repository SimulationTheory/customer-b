using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSE.Customer.Tests.Integration.TestObjects;
using PSE.Customer.V1.Clients.ClientProxy;
using Shouldly;

namespace PSE.Customer.Tests.Integration.V1.Clients
{
    [TestClass]
    public class ApiUserTests
    {
        [TestMethod]
        public void Constructor_JwtEncodedString_UsernameIsSet()
        {
            // Arrange
            var testUser = TestHelper.PaDev1;

            // Act
            var apiUser = new ApiUser(testUser.JwtEncodedString);

            // Assert
            var claim = apiUser.SecurityToken.Claims.FirstOrDefault(c => c.Type == "cognito:username");
            claim.ShouldNotBeNull();
            claim.Value.ShouldBe("testuserpadev1");
            apiUser.Username.ShouldBe("testuserpadev1");
        }

        [TestMethod]
        public void Constructor_JwtEncodedString_ContractAccountIdIsNotSet()
        {
            // Arrange
            var testUser = TestHelper.PaDev1;

            // Act
            var apiUser = new ApiUser(testUser.JwtEncodedString);

            // Assert
            apiUser.ContractAccountId.ShouldBe(0);
        }

        [TestMethod]
        public void Constructor_JwtEncodedString_BusinessPartnerIdIsSet()
        {
            // Arrange
            var testUser = TestHelper.PaDev1;

            // Act
            var apiUser = new ApiUser(testUser.JwtEncodedString);

            // Assert
            var claim = apiUser.SecurityToken.Claims.FirstOrDefault(c => c.Type == "custom:bp");
            claim.ShouldNotBeNull();
            claim.Value.ShouldBe("1002647070");
            apiUser.BPNumber.ShouldBe(1002647070);
        }

        [TestMethod]
        public void Constructor_JwtEncodedString_JwtTokenIsSet()
        {
            // Act
            var apiUser = new ApiUser(TestHelper.PaDev1JwtToken);

            // Assert
            apiUser.JwtEncodedString.ShouldBe(TestHelper.PaDev1JwtToken);
        }

        [TestMethod]
        public void Constructor_JwtEncodedString_SecurityTokenIsSet()
        {
            // Act
            var apiUser = new ApiUser(TestHelper.PaDev1JwtToken);

            // Assert
            apiUser.SecurityToken.RawData.ShouldBe(TestHelper.PaDev1JwtToken);
        }

        [TestMethod]
        public void SetJwtEncodedString_ValidJwtEncodedString_SetsAllFields()
        {
            // Arrange
            var apiUser = new ApiUser();

            // Act
            apiUser.SetJwtEncodedString(TestHelper.PaDev1JwtToken);

            // Assert
            apiUser.SecurityToken.RawData.ShouldBe(TestHelper.PaDev1JwtToken);
        }

        [TestMethod]
        public void GetClaim_JwtEncodedString_EmailClaimIsSet()
        {
            // Arrange
            var apiUser = new ApiUser(TestHelper.PaDev1JwtToken);

            // Act
            var claimValue = apiUser.GetClaimValue("email");

            // Assert
            claimValue.ShouldBe("testuserpaDev1@test.com");
        }

        [TestMethod]
        public void GetClaimValueAsDateTime_JwtEncodedString_AuthTimeClaimIsDecoded()
        {
            // Arrange
            var apiUser = new ApiUser(TestHelper.PaDev1JwtToken);

            // Act
            var claimValue = apiUser.GetClaimValueAsDateTime("auth_time");

            // Assert
            claimValue.ShouldNotBeNull();
            claimValue.Value.ShouldBe(new DateTime(2018, 3, 19, 22, 22, 38));
        }

        [TestMethod]
        public void AuthTime_JwtEncodedString_AuthTimeClaimIsDecoded()
        {
            // Arrange
            var apiUser = new ApiUser(TestHelper.PaDev1JwtToken);

            // Act
            var claimValue = apiUser.AuthTime;

            // Assert
            claimValue.ShouldNotBeNull();
            claimValue.Value.ShouldBe(new DateTime(2018, 3, 19, 22, 22, 38));
        }

        [TestMethod]
        public void ExpirationTime_JwtEncodedString_ExpirationTimeClaimIsDecoded()
        {
            // Arrange
            var apiUser = new ApiUser(TestHelper.PaDev1JwtToken);

            // Act
            var claimValue = apiUser.ExpirationTime;

            // Assert
            claimValue.ShouldNotBeNull();
            claimValue.Value.ShouldBe(new DateTime(2018, 3, 19, 23, 22, 38));
        }
    }
}
