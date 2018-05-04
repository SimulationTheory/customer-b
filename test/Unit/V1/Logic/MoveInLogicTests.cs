using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using PSE.Customer.V1.Clients.Address.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.Customer.V1.Logic;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Request;
using PSE.RestUtility.Core.Mcf;
using PSE.WebAPI.Core.Service.Enums;
using Shouldly;
using System;
using System.Collections.Generic;
using PSE.Customer.V1.Clients.Device.Interfaces;
using PSE.WebAPI.Core.Service.Interfaces;
using PSE.Customer.V1.Logic.Interfaces;

namespace PSE.Customer.Tests.Unit.V1.Logic
{
    [TestClass]
    public class MoveInLogicTests
    {
        private Mock<IMcfClient> _mcfClientMock;
        private Mock<ILogger<MoveInLogic>> _loggerMock;
        private Mock<IAddressApi> _addressApi;
        private Mock<IAccountApi> _accountApi;
        private Mock<IDeviceApi> _deviceApiMock;
        private Mock<IRequestContextAdapter> _requestContextMock;
        private Mock<ICustomerLogic> _customerMock;

        [TestInitialize]
        public void TestInitalize()
        {
            _mcfClientMock = new Mock<IMcfClient>();
            _loggerMock = new Mock<ILogger<MoveInLogic>>();
            _addressApi = new Mock<IAddressApi>();
            _accountApi = new Mock<IAccountApi>();
            _deviceApiMock = new Mock<IDeviceApi>();
            _requestContextMock = new Mock<IRequestContextAdapter>();
            _customerMock = new Mock<ICustomerLogic>();
        }

        [TestMethod]
        public void GetMoveInLatePayment_Returns_MoveInLatePaymentsResponse_Given_Valid_Input()
        {
            //Arrange
            var contractAccount = 11111111;
            var minPayment = 300m;
            var jwt = "token";
            var reconnect = false;

            _mcfClientMock.Setup(mcm => mcm.GetMoveInLatePaymentsResponse(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(
                    () => new MoveInLatePaymentsResponse()
                    {
                        AccountNo = contractAccount,
                        MinPayment = minPayment
                    });

            var target = new MoveInLogic(
                _loggerMock.Object,
                _mcfClientMock.Object,
                _addressApi.Object,
                _accountApi.Object,
                _deviceApiMock.Object,
                _customerMock.Object);

            //Act
            var actual = target.GetMoveInLatePayment(contractAccount, reconnect, jwt);

            //Assert
            actual.ShouldNotBeNull();
            actual.MinimumPaymentRequired.ShouldBe(minPayment);
        }

        [TestMethod]
        public void GetInvalidMoveinDates_Returns_GetInvalidMoveinDatesResponse_Given_Valid_Input()
        {
            //Arrange
            var sampleResponse = JsonConvert.DeserializeObject<McfResponse<GetHolidaysResponse>>(GetHolidaysResponse.GetSampleData());
            _mcfClientMock.Setup(mcm => mcm.GetInvalidMoveinDates(It.IsAny<GetInvalidMoveinDatesRequest>()))
                .Returns(sampleResponse);

            var target = new MoveInLogic(
                _loggerMock.Object,
                _mcfClientMock.Object,
                _addressApi.Object,
                _accountApi.Object,
                _deviceApiMock.Object,
                 _customerMock.Object);

            //Act
            var request = new GetInvalidMoveinDatesRequest
            {
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddMonths(1),
            };
            var actual = target.GetInvalidMoveinDates(request);

            //Assert
            actual.ShouldNotBeNull();
            actual.Count.ShouldBe(6);
        }

        [TestMethod]
        public void GetDuplicateBusinessPartnerIfExists_ReturnsBpSearchModel_GivenExistingBP()
        {
            //Arrange
            _mcfClientMock.Setup(mcm => mcm.GetDuplicateBusinessPartnerIfExists(It.IsAny<BpSearchRequest>()))
                .ReturnsAsync(
                    () => new BpSearchResponse()
                    {
                        BpId = "123456789",
                        BpSearchIdInfoSet = new McfList<IdentifierModel>()
                        {
                            Results = new List<IdentifierModel>()
                                            {
                                                new IdentifierModel()
                                                    {
                                                        IdentifierType = IdentifierType.ZLAST4,
                                                        IdentifierValue = "1234"
                                                    }
                                            }
                        },
                        Threshhold = " x",
                        Unique = "1 ",
                        Reason = "Because it matches.",
                        ReasonCode = "MATCH"
                    });

            var target = new MoveInLogic(
                _loggerMock.Object,
                _mcfClientMock.Object,
                _addressApi.Object,
                _accountApi.Object,
                _deviceApiMock.Object,
                 _customerMock.Object);

            //Act
            var actual = target.GetDuplicateBusinessPartnerIfExists(new BpSearchRequest());

            //Assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void GetDuplicateBusinessPartnerIfExists_ReturnsNull_IfMatchNotFound()
        {
            //Arrange
            _mcfClientMock.Setup(mcm => mcm.GetDuplicateBusinessPartnerIfExists(It.IsAny<BpSearchRequest>()))
                .ReturnsAsync(
                    () => new BpSearchResponse()
                    {
                        BpId = "123456789",
                        BpSearchIdInfoSet = new McfList<IdentifierModel>()
                        {
                            Results = new List<IdentifierModel>()
                            {
                                new IdentifierModel()
                                {
                                    IdentifierType = IdentifierType.ZLAST4,
                                    IdentifierValue = "1234"
                                }

                            }
                        },
                        Threshhold = "",
                        Unique = "0 ",
                        Reason = "Because it doesn't match.",
                        ReasonCode = "NO_MATCH"
                    });

            var target = new MoveInLogic(
                _loggerMock.Object,
                _mcfClientMock.Object,
                _addressApi.Object,
                _accountApi.Object,
                _deviceApiMock.Object,
                 _customerMock.Object);

            //Act
            var actual = target.GetDuplicateBusinessPartnerIfExists(new BpSearchRequest());

            //Assert
            actual.Result.MatchFound.ShouldBe(false);
        }
    }
}
