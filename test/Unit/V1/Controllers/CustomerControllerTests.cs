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
using PSE.Customer.Tests.Unit.TestObjects;
using PSE.Customer.V1.Clients.Mcf.Models;

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
                // !!! JMC - TODO: This logic fails intermittently.  It should only be initialized once.
                // No obvious solution.  Perhaps initializing elsewhere?  Maybe having an initialize and reset in using block?
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
            var actual = await target.GetCustomerProfileAsync(1001907289);

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
            var actual = await target.GetCustomerProfileAsync(1001907289);

            //Assert
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType<OkObjectResult>();
            var response = ((OkObjectResult)actual).Value as GetCustomerProfileResponse;
            response.ShouldNotBeNull();
            response.Phones.ToList().Count.ShouldBe(0);
        }
        [Ignore]
        [TestMethod]
        public async Task GetCustomerProfile_InvalidBpId_Returns500InternalServerError()
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
            results.ShouldBeOfType<ContentResult>();
            var returnCode = (ContentResult)results;
            returnCode.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        }
        [Ignore]
        [TestMethod]
        public async Task GetCustomerProfile_MissingClaims_Returns401UnauthorizedError()
        {
            //Arrange
            var target = GetController();
            ArrangeUserClaims(target, new List<Claim>());

            // Act
            var results = await target.GetCustomerProfileAsync();

            // Assert
            results.ShouldBeOfType<ContentResult>();
            var returnCode = (ContentResult)results;
            returnCode.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
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
            actual.ShouldBeOfType<StatusCodeResult>();
            (actual as StatusCodeResult).StatusCode.ShouldBe((int)HttpStatusCode.NoContent);
        }

        [TestMethod]
        public async Task LookupCustomer_UnhandledException_Returns500InternalServerError()
        {
            // Arrange
            const long acctId = 123456789012;
            const string testFullName = "JON SMITH";

            var lookupCustomerRequest = new LookupCustomerRequest
            {
                ContractAccountNumber = acctId,
                NameOnBill = testFullName,
            };

            CustomerLogicMock
                .Setup(clm => clm.LookupCustomer(It.IsAny<LookupCustomerRequest>()))
                .Throws(new ApplicationException("Customer lookup failed"));

            var target = GetController();

            //Act
            var actual = await target.LookupCustomer(lookupCustomerRequest);

            // Assert
            actual.ShouldBeOfType<ContentResult>();
            var returnCode = (ContentResult)actual;
            returnCode.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        }

        #endregion

        #region PutMailingAddressAsync Tests

        [TestMethod]
        public async Task PutMailingAddressAsync_ValidAddressAndClaim_ReturnsOk()
        {
            // Arrange
            var address = new AddressDefinedType
            {
                AddressLine1 = "The White House",
                AddressLine2 = "1600 Pennsylvania Avenue NW",
                City = "Washington",
                Country = "USA",
                PostalCode = "20500",
                State = "WA"
            };

            CustomerLogicMock.Setup(logic => logic.UpsertStandardMailingAddress(It.IsAny<long>(), It.IsAny<AddressDefinedType>(), It.IsAny<string>()))
                .Returns(() => 4131426);

            CustomerLogicMock.Setup(logic => logic.PutMailingAddressAsync(It.IsAny<AddressDefinedType>(), It.IsAny<long>()))
                .Returns(() => Task.FromResult(HttpStatusCode.OK));

            var controller = GetController();

            ArrangeController(controller, TestHelper.PaDev1);

            // Act
            var results = await controller.PutMailingAddressAsync(address);

            // Assert
            results.ShouldBeOfType<OkResult>();
        }

        [TestMethod]
        public async Task PutMailingAddressAsync_InvalidAddress_Returns400BadRequest()
        {
            // Arrange
            var address = new AddressDefinedType();
            var controller = GetController();
            controller.ViewData.Model = address;
            controller.ViewData.ModelState.AddModelError("AddressLine1", "AddressLine1 is required");

            // Act
            var results = await controller.PutMailingAddressAsync(address);

            // Assert
            results.ShouldBeOfType<BadRequestObjectResult>();
            var badRequest = (BadRequestObjectResult)results;
            badRequest.StatusCode.ShouldNotBeNull();
            badRequest.StatusCode.Value.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public async Task PutMailingAddressAsync_UnhandledException_Returns500InternalServerError()
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

            CustomerLogicMock.Setup(logic => logic.UpsertStandardMailingAddress(It.IsAny<long>(), It.IsAny<AddressDefinedType>(), It.IsAny<string>()))
                .Returns(() => 4131426);

            CustomerLogicMock.Setup(logic => logic.PutMailingAddressAsync(It.IsAny<AddressDefinedType>(), It.IsAny<long>()))
                .Throws(new ApplicationException("Batman is not available"));
            var controller = GetController();

            ArrangeController(controller, TestHelper.PaDev1);

            // Act
            var results = await controller.PutMailingAddressAsync(address);

            // Assert
            results.ShouldBeOfType<ContentResult>();
            var returnCode = (ContentResult) results;
            returnCode.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        }

        #endregion

        #region PutEmailAddressAsync Tests

        [TestMethod]
        public async Task PutEmailAddressAsync_ValidEmailAndClaim_ReturnsOk()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            CustomerLogicMock.Setup(logic => logic.PutEmailAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>()))
                .Returns(() => Task.FromResult(HttpStatusCode.OK));
            var controller = GetController();
            ArrangeController(controller, user);

            // Act
            var results = await controller.PutEmailAddressAsync(user.Email);

            // Assert
            results.ShouldBeOfType<OkResult>();
        }

        [TestMethod]
        public async Task PutEmailAddressAsync_EmailInvalidModelState_Returns400BadRequest()
        {
            // Arrange
            const string email = "";
            var controller = GetController();
            ArrangeController(controller, TestHelper.PaDev1);
            controller.ViewData.Model = email;
            controller.ViewData.ModelState.AddModelError("Email", "Invalid email format");

            // Act
            var results = await controller.PutEmailAddressAsync(email);

            // Assert
            results.ShouldBeOfType<BadRequestObjectResult>();
            var badRequest = (BadRequestObjectResult)results;
            badRequest.StatusCode.ShouldNotBeNull();
            badRequest.StatusCode.Value.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public async Task PutEmailAddressAsync_EmailFailsFailsRegExCheck_Returns400BadRequest()
        {
            // Arrange
            const string email = "test7 AT test DOT com";
            var controller = GetController();

            ArrangeController(controller, TestHelper.PaDev1);

            // Act
            var results = await controller.PutEmailAddressAsync(email);

            // Assert
            results.ShouldBeOfType<BadRequestObjectResult>();
            var badRequest = (BadRequestObjectResult)results;
            badRequest.StatusCode.ShouldNotBeNull();
            badRequest.StatusCode.Value.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public async Task PutEmailAddressAsync_UnhandledException_Returns500InternalServerError()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            CustomerLogicMock.Setup(logic => logic.PutEmailAddressAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>()))
                .Throws(new ApplicationException("Failed to update record"));
            var controller = GetController();
            ArrangeController(controller, user);

            // Act
            var results = await controller.PutEmailAddressAsync(user.Email);

            // Assert
            results.ShouldBeOfType<ContentResult>();
            var returnCode = (ContentResult)results;
            returnCode.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        }

        #endregion

        #region PutPhoneNumberAsync Tests

        [TestMethod]
        public async Task PutPhoneNumberAsync_ValidPhoneNumberAndClaim_ReturnsOk()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            CustomerLogicMock.Setup(logic => logic.PutPhoneNumberAsync(It.IsAny<string>(), It.IsAny<Phone>(), It.IsAny<long>()))
                .Returns(() => Task.FromResult(HttpStatusCode.OK));
            var controller = GetController();
            ArrangeController(controller, user);

            // Act
            var results = await controller.PutPhoneNumberAsync(user.Phones[0]);

            // Assert
            results.ShouldBeOfType<OkResult>();
        }

        [TestMethod]
        public async Task PutPhoneNumberAsync_InvalidPhoneNumber_Returns400BadRequest()
        {
            // Arrange
            var phone = new Phone { Type = PhoneType.Cell };
            var controller = GetController();
            ArrangeController(controller, TestHelper.PaDev1);
            controller.ViewData.Model = phone;
            controller.ViewData.ModelState.AddModelError("Number", "Number is required");

            // Act
            var results = await controller.PutPhoneNumberAsync(phone);

            // Assert
            results.ShouldBeOfType<BadRequestObjectResult>();
            var badRequest = (BadRequestObjectResult)results;
            badRequest.StatusCode.ShouldNotBeNull();
            badRequest.StatusCode.Value.ShouldBe(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public async Task PutPhoneNumberAsync_UnhandledException_Returns500InternalServerError()
        {
            // Arrange
            var user = TestHelper.PaDev1;
            CustomerLogicMock.Setup(logic => logic.PutPhoneNumberAsync(It.IsAny<string>(), It.IsAny<Phone>(), It.IsAny<long>()))
                .Throws(new ApplicationException("Failed to update record"));
            var controller = GetController();
            ArrangeController(controller, user);

            // Act
            var results = await controller.PutPhoneNumberAsync(user.Phones[0]);

            // Assert
            results.ShouldBeOfType<ContentResult>();
            var returnCode = (ContentResult)results;
            returnCode.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        }

        #endregion

        #region GetMailingAddressesAsync Tests

        [TestMethod]
        public async Task GetMailingAddressesAsync_Returns200OKTest()
        {
            //Arrange
            var user = TestHelper.PaDev1;
            CustomerLogicMock.Setup(dlm => dlm.GetMailingAddressesAsync(It.IsAny<long>(), It.IsAny<bool>(),It.IsAny<string>()))
                .Returns(() =>
                {
                    var mailingAddresses = GetMailingAddresses();
                    return Task.FromResult(mailingAddresses);
                });
            var controller = GetController();

            ArrangeController(controller, user);

            //Act
            var actual = await controller.GetMailingAddressesAsync(true);

            //Assert
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType<OkObjectResult>();
            ((OkObjectResult)actual).StatusCode.ShouldBe(StatusCodes.Status200OK);
            var response = ((OkObjectResult)actual).Value as GetMailingAddressesResponse;
            response.ShouldNotBeNull();
            response.MailingAddresses.ToList().Count.ShouldBeGreaterThan(0);
            response.MailingAddresses.First().Address.AddressLine1.ToString().ShouldBe("SE 166th St");

        }

        [TestMethod]
        public async Task GetMailingAddressesAsync_Returns404NoFoundTest()
        {
            //Arrange
            var user = TestHelper.PaDev1;
            CustomerLogicMock.Setup(dlm => dlm.GetMailingAddressesAsync(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(() =>
                {
                    IEnumerable<MailingAddressesModel> mailingAddresses = null;
                    return Task.FromResult(mailingAddresses);
                });
            var controller = GetController();

            ArrangeController(controller, user);

            //Act
            var actual = await controller.GetMailingAddressesAsync(true);

            //Assert
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType<NotFoundResult>();
            var response = (NotFoundResult)actual;
            response.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task GetMailingAddressesAsync_Returns401UnauthorizedTest()
        {
            //Arrange
            var target = GetController();
            target.ControllerContext = new ControllerContext();
            target.ControllerContext.HttpContext = new DefaultHttpContext();
            target.ControllerContext.HttpContext.Request.Headers["Authorization"] = "";

            //Act
            var actual = await target.GetMailingAddressesAsync(true);

            //Assert
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType<UnauthorizedResult>();

            var response = (UnauthorizedResult)actual;
            response.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        }

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

        private static IEnumerable<MailingAddressesModel> GetMailingAddresses()
        {
            return new List<MailingAddressesModel>
            {
                new MailingAddressesModel
                {
                    AddressID = 39403323,
                    Address = new AddressDefinedType
                    {
                            AddressLine1 ="SE 166th St",
                            AddressLine2 ="",
                            City ="Renton",
                            Country ="USA",
                            PostalCode ="98055-5107",
                            State="WA"
                    }
                },
                new MailingAddressesModel
                {
                    AddressID = 33343907,
                    Address = new AddressDefinedType
                    {
                         AddressLine1 ="350 110th Ave NE",
                         AddressLine2 ="",
                         City ="Bellevue",
                         Country ="USA",
                         PostalCode ="98004-1223",
                         State="WA"
                    }
                }

            };
        }

        #endregion
    }
}
