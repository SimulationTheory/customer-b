using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSE.Customer.Configuration;
using PSE.Customer.V1.Controllers;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Response;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PSE.Customer.Tests.Unit.V1.Controllers
{
    [TestClass]
    public class CustomerControllerTests
    {
        private Mock<IOptions<AppSettings>> AppSettingsMock { get; set; }
        private Mock<IDistributedCache> CacheMock { get; set; }
        private Mock<ILogger<CustomerController>> LoggerMock { get; set; }
        private Mock<ICustomerLogic> CustomerLogicMock { get; set; }

        #region Helper Methods

        private CustomerController GetController()
        {
            return new CustomerController(AppSettingsMock?.Object, CacheMock?.Object, LoggerMock?.Object,
                CustomerLogicMock?.Object);
        }

        private static void ArrangeUserClaims(ControllerBase target, IEnumerable<Claim> claims)
        {
            target.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "someAuthTypeName"))
                }
            };
        }

        [TestInitialize]
        public void TestInitialize()
        {
            AppSettingsMock = new Mock<IOptions<AppSettings>>();
            CacheMock = new Mock<IDistributedCache>();
            LoggerMock = new Mock<ILogger<CustomerController>>();
            CustomerLogicMock = new Mock<ICustomerLogic>();
        }

        #endregion

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            try
            {
                AutoMapper.Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<GetCustomerProfileResponse, CustomerProfileModel>();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region Constructor Tests

        [TestMethod]
        public void Constructor_AppSettingsIsNull_ExceptionThrown()
        {
            // Arrange
            AppSettingsMock = null;
            CustomerController controller = null;

            // Act
            Action action = () => controller = GetController();

            // Assert
            controller.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("appSettings");
        }

        [TestMethod]
        public void Constructor_CacheIsNull_ExceptionThrown()
        {
            // Arrange
            CacheMock = null;
            CustomerController controller = null;

            // Act
            Action action = () => controller = GetController();

            // Assert
            controller.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("cache");
        }

        [TestMethod]
        public void Constructor_LoggerIsNull_ExceptionThrown()
        {
            // Arrange
            LoggerMock = null;
            CustomerController controller = null;

            // Act
            Action action = () => controller = GetController();

            // Assert
            controller.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("logger");
        }

        [TestMethod]
        public void Constructor_CustomerLogicIsNull_ExceptionThrown()
        {
            // Arrange
            CustomerLogicMock = null;
            CustomerController controller = null;

            // Act
            Action action = () => controller = GetController();

            // Assert
            controller.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("customerLogic");
        }

        #endregion

        #region GetCustomerProfileAsync Tests

        [TestMethod]
        public async Task GetCustomerProfile_Test()
        {
            //Arrange

            CustomerLogicMock.Setup(dlm => dlm.GetCustomerProfileAsync(It.IsAny<long>()))
                .Returns(() =>
                {
                    var customerProfile = GetCustomerProfile();
                    return Task.FromResult(customerProfile);
                });

            var target = GetController();
            ArrangeUserClaims(target, new[]
            {
                new Claim("custom:bp", "1001907289")
            });

            //Act
            var actual = await target.GetCustomerProfileAsync();

            //Assert
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType<OkObjectResult>();
            var response = ((OkObjectResult)actual).Value as GetCustomerProfileResponse;
            response.ShouldNotBeNull();
            response.Phones.ToList().Count.ShouldBeGreaterThan(0);
            response.Phones.First().Type.ToString().ShouldBe(response.PrimaryPhone.ToString());
        }

        [TestMethod]
        public async Task GetCustomerProfile_NullPhone_Test()
        {
            //Arrange
            CustomerLogicMock.Setup(dlm => dlm.GetCustomerProfileAsync(It.IsAny<long>()))
                .Returns(() =>
                {
                    var customerProfile = GetCustomerProfile();
                    customerProfile.Phones = null;
                    return Task.FromResult(customerProfile);
                });

            var target = GetController();
            ArrangeUserClaims(target, new[]
            {
                new Claim("custom:bp", "1001907289")
            });

            //Act
            var actual = await target.GetCustomerProfileAsync();

            //Assert
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType<OkObjectResult>();
            var response = ((OkObjectResult)actual).Value as GetCustomerProfileResponse;
            response.ShouldNotBeNull();
            response.Phones.ToList().Count.ShouldBe(0);
        }

        [TestMethod]
        public async Task GetCustomerProfile_InvalidDataType_Returns500InternalServerError()
        {
            //Arrange
            var target = GetController();
            ArrangeUserClaims(target, new[]
            {
                new Claim("custom:bp", "abc")
            });

            // Act
            var results = await target.GetCustomerProfileAsync();

            // Assert
            results.ShouldBeOfType<StatusCodeResult>();
            var returnCode = (StatusCodeResult)results;
            returnCode.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        }

        #endregion

        #region LookupCustomer Tests

        [TestMethod]
        public async Task LookupCustomer_Test()
        {
            //Arrange
            const long bpId = 123456789;
            const long acctId = 123456789012;
            const bool hasWebAccount = true;
            const string testFullName = "JON SMITH";

            var lookupCustomerRequest = new LookupCustomerRequest
            {
                ContractAccountNumber = acctId,
                NameOnBill = testFullName,
            };

            CustomerLogicMock.Setup(clm => clm.LookupCustomer(It.IsAny<LookupCustomerRequest>()))
                .Returns((LookupCustomerRequest request) =>
                {
                    var lookupCustomerModel = new LookupCustomerModel
                    {
                        BPId = bpId,
                        HasWebAccount = hasWebAccount,
                    };

                    return Task.FromResult(lookupCustomerModel);
                });

            var target = GetController();

            //Act
            var actual = await target.LookupCustomer(lookupCustomerRequest);

            //Assert
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType<OkObjectResult>();
            var response = ((OkObjectResult)actual).Value as LookupCustomerResponse;
            response.ShouldNotBeNull();
            response.BPId.ShouldBe(bpId.ToString());
            response.HasWebAccount.ShouldBe(hasWebAccount);
        }

        [TestMethod]
        public async Task LookupCustomer_Bad_AcctId_Test()
        {
            //Arrange
            const long acctId = 123456789012;
            const string testFullName = "JON SMITH";

            var lookupCustomerRequest = new LookupCustomerRequest
            {
                ContractAccountNumber = acctId,
                NameOnBill = testFullName,
            };

            CustomerLogicMock
                .Setup(clm => clm.LookupCustomer(It.IsAny<LookupCustomerRequest>()))
                .Returns(Task.FromResult<LookupCustomerModel>(null));

            var target = GetController();

            //Act
            var actual = await target.LookupCustomer(lookupCustomerRequest);

            //Assert
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType<NotFoundResult>();
        }

        #endregion

        #region PutSaveMailingAddressAsync Tests

        [TestMethod]
        public async Task PutSaveMailingAddressAsync_ValidAddressAndClaim_ReturnsOk()
        {
            // Arrange
            var address = new AddressDefinedType
            {
                AddressLine1 = "The White House",
                AddressLine2 = "1600 Pennsylvania Avenue NW",
                City = "Washington",
                Country = "USA",
                PostalCode = "20500"
            };
            CustomerLogicMock.Setup(logic => logic.PutSaveMailingAddressAsync(It.IsAny<AddressDefinedType>(), It.IsAny<long>()))
                .Returns(() => Task.FromResult(HttpStatusCode.OK));
            var controller = GetController();
            ArrangeUserClaims(controller, new[]
            {
                new Claim("custom:bp", "1001907289")
            });

            // Act
            var results = await controller.PutSaveMailingAddressAsync(address);

            // Assert
            results.ShouldBeOfType<OkResult>();
        }

        [TestMethod]
        public async Task PutSaveMailingAddressAsync_InvalidAddress_ReturnsOk()
        {
            // Arrange
            var address = new AddressDefinedType();
            var controller = GetController();
            controller.ViewData.Model = address;
            controller.ViewData.ModelState.AddModelError("AddressLine1", "AddressLine1 is required");

            // Act
            var results = await controller.PutSaveMailingAddressAsync(address);

            // Assert
            results.ShouldBeOfType<BadRequestObjectResult>();
            var badRequest = (BadRequestObjectResult)results;
            badRequest.StatusCode.ShouldNotBeNull();
            badRequest.StatusCode.Value.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public async Task PutSaveMailingAddressAsync_PutFails_Returns500InternalServerError()
        {
            // Arrange
            var address = new AddressDefinedType
            {
                AddressLine1 = "Bruce Wayne",
                AddressLine2 = "1007 Mountain Drive",
                City = "Gotham",
                Country = "USA",
                PostalCode = "10001"
            };
            CustomerLogicMock.Setup(logic => logic.PutSaveMailingAddressAsync(It.IsAny<AddressDefinedType>(), It.IsAny<long>()))
                .Throws(new ApplicationException("Batman is not available"));
            var controller = GetController();
            ArrangeUserClaims(controller, new[]
            {
                new Claim("custom:bp", "1001907289")
            });

            // Act
            var results = await controller.PutSaveMailingAddressAsync(address);

            // Assert
            results.ShouldBeOfType<StatusCodeResult>();
            var returnCode = (StatusCodeResult) results;
            returnCode.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        }

        #endregion

        #region PutSaveEmailAddressAsync Tests

        // TBD

        #endregion

        #region PutSavePhoneNumbersAsync Tests

        // TBD

        #endregion

        #region Private Methods

        private static CustomerProfileModel GetCustomerProfile()
        {
            return new CustomerProfileModel
            {
                EmailAddress = "test@pse.com",
                MailingAddress = new AddressDefinedType
                {
                    AddressLine1 = "350 110th Ave NE",
                    AddressLine2 = string.Empty,
                    City = "Bellevue",
                    State = "WA",
                    Country = "USA",
                    PostalCode = "98004-1223"
                },
                Phones = new List<Phone>
                {
                    new Phone {Type = PhoneType.Cell, Number="4251234567"},
                    new Phone {Type= PhoneType.Home, Number="5251234567"},
                    new Phone {Type= PhoneType.Work, Number="6251234567", Extension="1234"}
                },
                PrimaryPhone = PhoneType.Cell
            };
        }

        #endregion
    }

}
