﻿using Microsoft.AspNetCore.Http;
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
using PSE.Customer.V1.Repositories.Entities;
using PSE.Customer.V1.Response;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PSE.Customer.Tests.Unit.V1.Controllers
{
    [TestClass]
    public class CustomerControllerTests
    {
        private readonly Mock<ICustomerLogic> _customerLogicMock;
        private readonly Mock<IOptions<AppSettings>> _appSettingsMock;
        private readonly Mock<IDistributedCache> _cacheMock;
        private readonly Mock<ILogger<CustomerController>> _loggerMock;

        public CustomerControllerTests()
        {
            _loggerMock = new Mock<ILogger<CustomerController>>();
            _customerLogicMock = new Mock<ICustomerLogic>();
            _appSettingsMock = new Mock<IOptions<AppSettings>>();
            _cacheMock = new Mock<IDistributedCache>();
        }


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<GetCustomerProfileResponse, CustomerProfileModel>();
            });
        }


        [TestMethod]
        public async Task GetCustomerProfile_Test()
        {
            //Arrange

            _customerLogicMock.Setup(dlm => dlm.GetCustomerProfileAsync(It.IsAny<long>()))
                .Returns(() =>
                {
                    var customerProfile = GetCustomerProfile();
                    return Task.FromResult(customerProfile);
                });
           
            var target = new CustomerController(_appSettingsMock.Object, _cacheMock.Object, _loggerMock.Object, _customerLogicMock.Object);

            target.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                       new Claim("custom:bp", "1001907289")
                    }, "someAuthTypeName"))
                }
            };

            //Act
            var actual = await target.GetCustomerProfileAsync();

            //Assert
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType<OkObjectResult>();
            var response = ((OkObjectResult)actual).Value as GetCustomerProfileResponse;
            response.ShouldNotBeNull();
            response.Phones.ToList().Count().ShouldBeGreaterThan(0);
            response.Phones.First().Type.ToString().ShouldBe(response.PrimaryPhone.ToString());
        }


        [TestMethod]
        public async Task GetCustomerProfile_NullPhone_Test()
        {
            //Arrange

            _customerLogicMock.Setup(dlm => dlm.GetCustomerProfileAsync(It.IsAny<long>()))
                .Returns(() =>
                {
                    var customerProfile = GetCustomerProfile();
                    customerProfile.Phones = null;
                    return Task.FromResult(customerProfile);
                });

            var target = new CustomerController(_appSettingsMock.Object, _cacheMock.Object, _loggerMock.Object, _customerLogicMock.Object);

            target.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                       new Claim("custom:bp", "1001907289")
                    }, "someAuthTypeName"))
                }
            };

            //Act
            var actual = await target.GetCustomerProfileAsync();

            //Assert
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType<OkObjectResult>();
            var response = ((OkObjectResult)actual).Value as GetCustomerProfileResponse;
            response.ShouldNotBeNull();
            response.Phones.ToList().Count().ShouldBe(0);
        }

        [TestMethod]
        public async Task GetCustomerProfile_InvalidDtaTypeThrowsException_Test()
        {
            //Arrange

            string expectedExceptionMessage = "abc should be Long data type";

            _customerLogicMock.Setup(dlm => dlm.GetCustomerProfileAsync(It.IsAny<long>()))
                .Returns(() =>
                {
                    var customerProfile = GetCustomerProfile();
                    return Task.FromResult(customerProfile);
                });

            var target = new CustomerController(_appSettingsMock.Object, _cacheMock.Object, _loggerMock.Object, _customerLogicMock.Object);

            target.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                       new Claim("custom:bp", "abc")
                    }, "someAuthTypeName"))
                }
            };

            try
            {
                //Act
                var actual = await target.GetCustomerProfileAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Assert
                ex.Message.ShouldBe(expectedExceptionMessage);
            }
        }

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

            _customerLogicMock.Setup(clm => clm.LookupCustomer(It.IsAny<LookupCustomerRequest>()))
                .Returns((LookupCustomerRequest request) =>
                {
                    var lookupCustomerModel = new LookupCustomerModel
                    {
                        BPId = bpId,
                        HasWebAccount = hasWebAccount,
                    };

                    return Task.FromResult(lookupCustomerModel);
                });

            var target = new CustomerController(_appSettingsMock.Object, _cacheMock.Object, _loggerMock.Object, _customerLogicMock.Object);

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

            _customerLogicMock
                .Setup(clm => clm.LookupCustomer(It.IsAny<LookupCustomerRequest>()))
                .Returns(Task.FromResult<LookupCustomerModel>(null));

            var target = new CustomerController(_appSettingsMock.Object, _cacheMock.Object, _loggerMock.Object, _customerLogicMock.Object);

            //Act
            var actual = await target.LookupCustomer(lookupCustomerRequest);

            //Assert
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType<NotFoundResult>();
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
                Phones = new List<Phone>()
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
