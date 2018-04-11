using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSE.Customer.Configuration;
using PSE.Customer.Tests.Unit.TestObjects;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Controllers;
using PSE.Customer.V1.Logic.Interfaces;
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
        #endregion

        #region CreateIDType Tests
        #endregion

        #region UpdateIDType Tests
        #endregion

        #endregion
    }
}
