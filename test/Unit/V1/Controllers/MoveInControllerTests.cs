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
    public class MoveInControllerTests
    {
        private Mock<ILogger<MoveInController>> LoggerMock { get; set; }
        private Mock<IMoveInLogic> MoveInLogicMock { get; set; }
        private Mock<IOptions<AppSettings>> AppSettingsMock { get; set; }

        #region Helpers

        private MoveInController GetController()
        {
            return new MoveInController(
                AppSettingsMock?.Object,
                LoggerMock?.Object,
                MoveInLogicMock?.Object);
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
            MoveInLogicMock = new Mock<IMoveInLogic>();
            AppSettingsMock = new Mock<IOptions<AppSettings>>();
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
            var user = TestHelper.PaDev1;
            MoveInLogicMock.
                Setup(x => x.GetAllIdTypes(It.IsAny<long>())).
                Returns(Task.FromResult(new List<IdentifierTypeResponse>
                {
                    new IdentifierTypeResponse
                    {
                        IdentifierType = IdentifierType.ZLAST4
                    },
                    new IdentifierTypeResponse
                    {
                        IdentifierType = IdentifierType.ZDOB
                    }
                }));
            var controller = GetController();
            ArrangeController(controller, user);

            // Act
            var response = controller.GetAllIdTypes();

            // Assert
            var result = (OkObjectResult)response.Result;
            var idResponse = (GetBpIdTypeResponse)result.Value;
            idResponse.Identifiers.Count.ShouldBe(2);
            idResponse.Identifiers[0].IdentifierType.ShouldBe(IdentifierType.ZLAST4);
            idResponse.Identifiers[1].IdentifierType.ShouldBe(IdentifierType.ZDOB);
        }

        #endregion

        #region GetIdType Tests

        [TestMethod]
        public void GetIdType_ValidAccount_ReturnsValue()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            MoveInLogicMock.
                Setup(x => x.GetIdType(It.IsAny<long>(), It.IsAny<IdentifierType>())).
                Returns(Task.FromResult(new List<IdentifierTypeResponse>
                {
                    new IdentifierTypeResponse
                    {
                        IdentifierType = IdentifierType.ZDOB
                    }
                }));
            var controller = GetController();
            ArrangeController(controller, user);

            // Act
            var response = controller.GetIdType(IdentifierType.ZDOB);

            // Assert
            var result = (OkObjectResult)response.Result;
            var idResponse = (GetBpIdTypeResponse)result.Value;
            idResponse.Identifiers.Count.ShouldBe(1);
        }

        #endregion

        #region CreateIdType Tests

        [TestMethod]
        public async Task CreateIdType_ValidAccountAndType_SavedSuccessfully()
        {
            // Arrange
            var controller = GetController();
            var identifier = new IdentifierRequest();

            // Act
            var response = await controller.CreateIdType(identifier);

            // Assert
            response.ShouldBeOfType<OkObjectResult>();
        }

        #endregion

        #region UpdateIdType Tests

        [TestMethod]
        public void UpdateIdType_ValidAccountAndType_SavedSuccessfully()
        {
            // Arrange
            var controller = GetController();
            var identifier = new IdentifierRequest();

            // Act
            var response = controller.UpdateIdType(identifier);

            // Assert
            response.ShouldBeOfType<OkResult>();
        }

        #endregion

        #region ValidateIdType Tests

        [TestMethod]
        public void ValidateType_ValidAccountAndType_TrueReturned()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            MoveInLogicMock.
                Setup(x => x.ValidateIdType(It.IsAny<IdentifierRequest>())).
                Returns(Task.FromResult(true));
            var controller = GetController();
            var identifier = new IdentifierRequest
            {
                BpId = user.BpNumber.ToString(),
                IdentifierType = IdentifierType.ZLAST4,
                IdentifierNo = "9999"
            };

            // Act
            var response = controller.ValidateIdType(identifier);

            // Assert
            response.Result.ShouldBeOfType(typeof(OkObjectResult));
            var result = (ValidateIdTypeResponse)((OkObjectResult)response.Result).Value;
            result.ShouldNotBeNull();
            result.PiiMatch.ShouldBe("Y");
        }

        [TestMethod]
        public void ValidateType_InvalidAccountAndType_FalseReturned()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            MoveInLogicMock.
                Setup(x => x.ValidateIdType(It.IsAny<IdentifierRequest>())).
                Returns(Task.FromResult(false));
            var controller = GetController();
            var identifier = new IdentifierRequest
            {
                BpId = user.BpNumber.ToString(),
                IdentifierType = IdentifierType.ZLAST4,
                IdentifierNo = "1234"
            };

            // Act
            var response = controller.ValidateIdType(identifier);

            // Assert
            response.Result.ShouldBeOfType(typeof(OkObjectResult));
            var result = (ValidateIdTypeResponse)((OkObjectResult)response.Result).Value;
            result.ShouldNotBeNull();
            result.PiiMatch.ShouldBe("N");
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
            result.ShouldNotBeNull();
            result.InvalidMoveinDates.ShouldNotBeNull();
            result.InvalidMoveinDates.Count.ShouldBe(5);
        }

        #region GetDuplicateBusinessPartnerIfExists Tests
        [TestMethod]
        public void GetDuplicateBusinessPartnerIfExists_MissingParams_ReturnsBadRequest()
        {
            // Arrange
            var controller = GetController();
            var request = new BpSearchRequest()
            {
                FirstName = "Feng",
            };

            // Act
            var response = controller.BpSearch(request).Result;

            // Assert
            response.ShouldBeOfType<BadRequestObjectResult>();

        }

        [TestMethod]
        public void GetDuplicateBusinessPartnerIfExists_ExistingBP_Returns200Ok()
        {
            // Arrange
            var controller = GetController();
            var responseForExistingBp = new BpSearchModel()
            {
                MatchFound = true,
                BpId = 1234567,
                Reason = "Valid match.",
                ReasonCode = "match",
                BpSearchIdentifiers = new List<IdentifierModel>()
                                            {
                                                new IdentifierModel()
                                                    {
                                                        IdentifierType = IdentifierType.ZDOB,
                                                        IdentifierValue = "01/01/1975"
                                                    }
                                            }
            };
            MoveInLogicMock.Setup(m => m.GetDuplicateBusinessPartnerIfExists(It.IsAny<BpSearchRequest>()))
                .Returns(responseForExistingBp);
            var request = new BpSearchRequest()
            {
                FirstName = "Feng",
                LastName = "Chan"
            };

            // Act
            var response = controller.BpSearch(request).Result;

            // Assert
            response.ShouldBeOfType<OkObjectResult>();
        }

        #endregion
    }
}
