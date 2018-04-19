using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSE.Customer.Tests.Unit.TestObjects;
using PSE.Customer.V1.Controllers;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using Shouldly;

namespace PSE.Customer.Tests.Unit.V1.Controllers
{
    [TestClass]
    public class MoveInControllerTests
    {
        private Mock<ILogger<MoveInController>> LoggerMock { get; set; }
        private Mock<IMoveInLogic> MoveInLogicMock { get; set; }

        #region Helpers

        private MoveInController GetController()
        {
            return new MoveInController(LoggerMock?.Object, MoveInLogicMock.Object);
        }

        // Likely needed later.  Delete if not needed by 04/21
        private static void ArrangeUserClaims(ControllerBase controller, IEnumerable<Claim> claims)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "someAuthTypeName"))
                }
            };
        }

        // Likely needed later.  Delete if not needed by 04/21
        private static void ArrangeController(ControllerBase controller, TestUser user)
        {
            if (controller.ControllerContext != null)
            {
                var claims = new[]
                {
                    new Claim("custom:bp", user.BpNumber.ToString())
                };
                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(claims, "someAuthTypeName"))
                    }
                };
                controller.ControllerContext.HttpContext.Request.Headers.Add("Authorization", user.JwtToken);
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            LoggerMock = new Mock<ILogger<MoveInController>>();
            MoveInLogicMock = new Mock<IMoveInLogic>();
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_LoggerIsNull_ExceptionThrown()
        {
            // Arrange
            LoggerMock = null;
            MoveInController controller = null;

            // Act
            Action action = () => controller = GetController();

            // Assert
            controller.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("logger");
        }

        #endregion

        #region Business Partner ID Type Tests

        #region GetAllIdTypes Tests

        [TestMethod]
        public void GetAllIdTypes_ValidAccount_ReturnsValues()
        {
            // Arrange
            var controller = GetController();

            // Act
            var response = controller.GetAllIdTypes();

            // Assert
            var result = (OkObjectResult)response.Result;
            var idResponse = (IndentifierResponse) result.Value;
            idResponse.Identifiers.Count.ShouldBeGreaterThan(0);
        }

        #endregion

        #region GetIdType Tests

        [TestMethod]
        public void GetIdType_ValidAccount_ReturnsValue()
        {
            // Arrange
            var controller = GetController();

            // Act
            var response = controller.GetIdType(IdentifierType.ZDOB);

            // Assert
            var result = (OkObjectResult)response.Result;
            var idResponse = (IndentifierResponse)result.Value;
            idResponse.Identifiers.Count.ShouldBeGreaterThan(0);
        }

        #endregion

        #region CreateIDType Tests

        [TestMethod]
        public void CreateIDType_ValidAccountAndType_SavedSuccessfully()
        {
            // Arrange
            var controller = GetController();
            var identifier = new IdentifierRequest();

            // Act
            var response = controller.CreateIdType(identifier);

            // Assert
            response.Result.ShouldBeOfType<OkResult>();
        }

        #endregion

        #region UpdateIDType Tests

        [TestMethod]
        public void UpdateIDType_ValidAccountAndType_SavedSuccessfully()
        {
            // Arrange
            var controller = GetController();
            var identifier = new IdentifierRequest();

            // Act
            var response = controller.UpdateIdType(identifier);

            // Assert
            response.Result.ShouldBeOfType<OkResult>();
        }

        #endregion

        #endregion

        [TestMethod]
        public void GetInvalidMoveinDates_Valid_Request_Returns_Valid_Response()
        {
            // Arrange
            var dateList = new List<DateTimeOffset>();
            for (int i = 0; i < 5; i++)
            {
                dateList.Add(DateTime.Now.AddDays(i));
            }

            MoveInLogicMock.Setup(m => m.GetInvalidMoveinDates(It.IsAny<GetInvalidMoveinDatesRequest>())).Returns(dateList);
            var controller = GetController();

            var request = new GetInvalidMoveinDatesRequest
            {
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddMonths(3),
            };

            // Act
            var response = controller.GetInvalidMoveinDates(request);

            // Assert
            var result = ((OkObjectResult)response.Result).Value as GetInvalidMoveinDatesResponse;
            result.InvalidMoveinDates.ShouldNotBeNull();
            result.InvalidMoveinDates.Count.ShouldBe(5);
        }
    }
}
