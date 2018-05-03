using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using PSE.Customer.V1.Clients.Account.Models.Response;
using PSE.Customer.V1.Clients.Address.Interfaces;
using PSE.Customer.V1.Clients.Device.Interfaces;
using PSE.Customer.V1.Clients.Device.Models;
using PSE.Customer.V1.Clients.Device.Models.Response;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.Customer.V1.Logic;
using PSE.Customer.V1.Request;
using PSE.RestUtility.Core.Mcf;
using PSE.WebAPI.Core.Service.Interfaces;
using RestSharp;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSE.Customer.Tests.Unit.V1.Logic
{
    [TestClass]
    public class MoveOutLogicTests
    {
        private Mock<IMcfClient> _mcfClientMock;
        private Mock<ILogger<MoveInLogic>> _loggerMock;
        private Mock<IAddressApi> _addressApi;
        private Mock<IDeviceApi> _deviceApiMock;
        private Mock<IAccountApi> _accountApiMock;
        private Mock<IRequestContextAdapter> _requestContextMock;

        [TestInitialize]
        public void TestInitalize()
        {
            _mcfClientMock = new Mock<IMcfClient>();
            _loggerMock = new Mock<ILogger<MoveInLogic>>();
            _addressApi = new Mock<IAddressApi>();
            _deviceApiMock = new Mock<IDeviceApi>();
            _accountApiMock = new Mock<IAccountApi>();
            _requestContextMock = new Mock<IRequestContextAdapter>();
        }

        #region Stop Service
        [TestMethod]
        public async Task StopService_Returns_GetMoveoutStopServiceResponse_Given_Valid_Input()
        {
            //Arrange
            string installationGuid = "mockinstallationguid";

            _accountApiMock.Setup(cam => cam.GetContractItems(It.IsAny<long>())).Returns(Task.FromResult(GetAccountResponse(installationGuid)));

            _deviceApiMock.Setup(dm => dm.GetInstallationDetail(It.IsAny<long>())).Returns(Task.FromResult(GetDeviceResponse(installationGuid)));

            var sampleResponse = JsonConvert.DeserializeObject<McfResponse<GetContractItemMcfResponse>>(GetContractItemMcfResponse.GetSampleData());
            _mcfClientMock.Setup(mcm => mcm.StopService(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<DateTimeOffset>()))
                .Returns(sampleResponse);

            var target = CreateTarget();

            //Act
            var actual = await target.StopService(GetMoveOutStopServiceRequest());

            //Assert
            actual.ShouldNotBeNull();
            actual.Status.Count.ShouldBe(2);
        }

        [TestMethod]
        public async Task StopService_Returns_Null_When_ContractAccountId_Not_Found()
        {
            //Arrange
            string installationGuid = "mockinstallationguid";
            var accountResponse = GetAccountResponse(installationGuid);
            accountResponse.ContractItems.Clear();

            _accountApiMock.Setup(cam => cam.GetContractItems(It.IsAny<long>())).Returns(Task.FromResult(accountResponse));
            _deviceApiMock.Setup(dm => dm.GetInstallationDetail(It.IsAny<long>())).Returns(Task.FromResult(GetDeviceResponse(installationGuid)));

            var sampleResponse = JsonConvert.DeserializeObject<McfResponse<GetContractItemMcfResponse>>(GetContractItemMcfResponse.GetSampleData());
            _mcfClientMock.Setup(mcm => mcm.StopService(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<DateTimeOffset>()))
                .Returns(sampleResponse);

            var target = CreateTarget();

            //Act
            var actual = await target.StopService(GetMoveOutStopServiceRequest());

            //Assert
            actual.ShouldBeNull();
        }

        [TestMethod]
        public async Task StopService_Returns_Null_With_No_InstallationIds()
        {
            //Arrange
            var request = GetMoveOutStopServiceRequest();
            request.InstallationIds.Clear();

            var target = CreateTarget();

            //Act
            var actual = await target.StopService(request);

            //Assert
            actual.ShouldBeNull();
        }

        [TestMethod]
        public async Task StopService_Returns_GetMoveoutStopServiceResponse_With_Invalid_InstallationIds()
        {
            //Arrange
            string installationGuid = "mockinstallationguid";
            var deviceResponse = GetDeviceResponse("anothermockinstallationguid");
            deviceResponse.Data = null;

            _accountApiMock.Setup(cam => cam.GetContractItems(It.IsAny<long>())).Returns(Task.FromResult(GetAccountResponse(installationGuid)));
            _deviceApiMock.Setup(dm => dm.GetInstallationDetail(It.IsAny<long>())).Returns(Task.FromResult(deviceResponse));

            var sampleResponse = JsonConvert.DeserializeObject<McfResponse<GetContractItemMcfResponse>>(GetContractItemMcfResponse.GetSampleData());
            _mcfClientMock.Setup(mcm => mcm.StopService(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<DateTimeOffset>()))
                .Returns(sampleResponse);

            var target = CreateTarget();

            //Act
            var actual = await target.StopService(GetMoveOutStopServiceRequest());

            //Assert
            actual.ShouldNotBeNull();
            actual.Status.Count.ShouldBe(2);
        }

        [TestMethod]
        public async Task StopService_Returns_GetMoveoutStopServiceResponse_When_InstallationGuids_Do_Not_Match()
        {
            //Arrange
            string installationGuid = "mockinstallationguid";

            _accountApiMock.Setup(cam => cam.GetContractItems(It.IsAny<long>())).Returns(Task.FromResult(GetAccountResponse(installationGuid)));
            _deviceApiMock.Setup(dm => dm.GetInstallationDetail(It.IsAny<long>())).Returns(Task.FromResult(GetDeviceResponse("mockBADinstallationguid")));

            var sampleResponse = JsonConvert.DeserializeObject<McfResponse<GetContractItemMcfResponse>>(GetContractItemMcfResponse.GetSampleData());
            _mcfClientMock.Setup(mcm => mcm.StopService(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<DateTimeOffset>()))
                .Returns(sampleResponse);

            var target = CreateTarget();

            //Act
            var actual = await target.StopService(GetMoveOutStopServiceRequest());

            //Assert
            actual.ShouldNotBeNull();
            actual.Status.Count.ShouldBe(2);
        }

        [TestMethod]
        public async Task StopService_Returns_GetMoveoutStopServiceResponse_When_Mcf_Stop_Service_Fails()
        {
            //Arrange
            string installationGuid = "mockinstallationguid";

            _accountApiMock.Setup(cam => cam.GetContractItems(It.IsAny<long>())).Returns(Task.FromResult(GetAccountResponse(installationGuid)));
            _deviceApiMock.Setup(dm => dm.GetInstallationDetail(It.IsAny<long>())).Returns(Task.FromResult(GetDeviceResponse(installationGuid)));

            var sampleResponse = JsonConvert.DeserializeObject<McfResponse<GetContractItemMcfResponse>>(GetContractItemMcfResponse.GetSampleData());
            sampleResponse.Result = null;
            sampleResponse.Error = new McfErrorResult
            {
                Code = "Some mcf code:666",
            };
            sampleResponse.Error.Message = new McfErrorMessage
            {
                Value = "Some mcf error message."
            };

            _mcfClientMock.Setup(mcm => mcm.StopService(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<DateTimeOffset>()))
                .Returns(sampleResponse);

            var target = CreateTarget();

            //Act
            var actual = await target.StopService(GetMoveOutStopServiceRequest());

            //Assert
            actual.ShouldNotBeNull();
            actual.Status.Count.ShouldBe(2);
        }
        #endregion


        #region Stop Service Test Helpers
        private MoveOutStopServiceRequest GetMoveOutStopServiceRequest()
        {
            return new MoveOutStopServiceRequest
            {
                ContractAccountId = 12345678901,
                MoveOutDate = DateTimeOffset.Now,
                InstallationIds = new List<long>
                {
                    12345678901, 9876542101
                }
            };
        }

        private IRestResponse<GetInstallationResponse>  GetDeviceResponse(string installationGuid)
        {
            var deviceContent = new GetInstallationResponse
            {
                Installation = new Installation
                {
                    InstallationGuid = installationGuid,
                }
            };

            return new RestResponse<GetInstallationResponse>
            {
                Content = JsonConvert.SerializeObject(deviceContent),
                Data = deviceContent,
            };
        }

        private GetContractItemsResponse GetAccountResponse(string installationGuid)
        {
            return new GetContractItemsResponse
            {
                ContractItems = new List<GetContractItemResponse>
                {
                    { new GetContractItemResponse { PointOfDeliveryGuid = installationGuid} }
                }
            };
        }
        #endregion

        private MoveOutLogic CreateTarget()
        {
            return new MoveOutLogic(
            _loggerMock.Object,
            _mcfClientMock.Object,
            _addressApi.Object,
            _accountApiMock.Object,
            _deviceApiMock.Object,
            _requestContextMock.Object);
        }
    }
}
