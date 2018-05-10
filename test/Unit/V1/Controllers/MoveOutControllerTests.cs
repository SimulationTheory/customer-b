using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSE.Customer.Configuration;
using PSE.Customer.Tests.Unit.TestObjects;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Controllers;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PSE.Customer.Tests.Unit.V1.Controllers
{

    [TestClass]
    public class MoveOutControllerTests
    {
        private Mock<ILogger<MoveInController>> LoggerMock { get; set; }
        private Mock<IMoveOutLogic> MoveOutLogicMock { get; set; }
        private Mock<IOptions<AppSettings>> AppSettingsMock { get; set; }

        #region Helpers

        private MoveOutController GetController()
        {
            return new MoveOutController(
                AppSettingsMock?.Object,
                LoggerMock?.Object,
                MoveOutLogicMock?.Object);
        }

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
            MoveOutLogicMock = new Mock<IMoveOutLogic>();
            AppSettingsMock = new Mock<IOptions<AppSettings>>();
        }

        #endregion

        #region Constructor Tests

        [TestMethod]
        public void Constructor_LoggerIsNull_ExceptionThrown()
        {
            // Arrange
            LoggerMock = null;
            MoveOutController controller = null;

            // Act
            Action action = () => controller = GetController();

            // Assert
            controller.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("logger");
        }

        #endregion

        [TestMethod]
        public void PostStopService_Valid_Request_Returns_Valid_Response()
        {
            // Arrange
            var logicResonse = new StopServiceResponse
            {
                WarmHomeFund = 123.45M,
                FinalBillDate = DateTimeOffset.Now,
                FinalBillDueDate = DateTimeOffset.Now.AddDays(15),
                Status = new Dictionary<long, string>
                {
                    { 12345678901, "Some status message 1" },
                    { 10987654321, "Some status message 2" }
                }
            };

            MoveOutLogicMock.Setup(lm => lm.StopService(It.IsAny<MoveOutStopServiceRequest>())).Returns(Task.FromResult(logicResonse));
            var controller = GetController();

            var request = new MoveOutStopServiceRequest
            {
                ContractAccountId = 12345678901,
                MoveOutDate = DateTimeOffset.Now,
                InstallationIds = new List<long>
                {
                    12345678901, 10987654321
                }
            };

            // Act
            var response = controller.PostStopService(request);

            // Assert
            response.Result.ShouldBeOfType<OkObjectResult>();
            var result = ((OkObjectResult)response.Result).Value as StopServiceResponse;
            result.ShouldNotBeNull();
            result.Status.Count.ShouldBe(2);
        }

        [TestMethod]
        public void PostStopService_InValid_Request_Returns_NotFound_Response()
        {
            // Arrange
            StopServiceResponse logicResonse = null;
            MoveOutLogicMock.Setup(lm => lm.StopService(It.IsAny<MoveOutStopServiceRequest>())).Returns(Task.FromResult(logicResonse));
            var controller = GetController();

            var request = new MoveOutStopServiceRequest
            {
                ContractAccountId = 12345678901,
                MoveOutDate = DateTimeOffset.Now,
                InstallationIds = new List<long>
                {
                    12345678901, 10987654321
                }
            };

            // Act
            var response = controller.PostStopService(request);

            // Assert
            response.Result.ShouldBeOfType<NotFoundResult>();
        }
    }
}
