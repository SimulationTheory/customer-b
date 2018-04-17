using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSE.Customer.V1.Clients.Address.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Logic;
using PSE.Customer.V1.Models;
using Shouldly;

namespace PSE.Customer.Tests.Unit.V1.Logic
{
    [TestClass]
    public class MoveInLogicTests
    {

        private Mock<IMcfClient> _mcfClientMock;
        private Mock<ILogger<MoveInLogic>> _loggerMock;
        private Mock<IAddressApi> _addressApi;

        [TestInitialize]
        public void TestInitalize()
        {
            _mcfClientMock = new Mock<IMcfClient>();
            _loggerMock = new Mock<ILogger<MoveInLogic>>();
            _addressApi = new Mock<IAddressApi>();
        }

        [TestMethod]
        public void GetMoveInLatePayment_Returns_MoveInLatePaymentsResponse_Given_Valid_Input()
        {
            //Arrange
            var contractAccount = 11111111;
            var minPayment = 300m;
            var jwt = "token";
            

            _mcfClientMock.Setup(mcm => mcm.GetMoveInLatePaymentsResponse(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(
                    () => new MoveInLatePaymentsResponse()
                    {
                        AccountNo = contractAccount,
                        MinPayment = minPayment
                    });
            
            var target = new MoveInLogic(_loggerMock.Object, _mcfClientMock.Object, _addressApi.Object);

            //Act
            var actual = target.GetMoveInLatePayment(contractAccount, jwt);

            //Assert
            actual.ShouldNotBeNull();
            actual.MinimumPaymentRequired.ShouldBe(minPayment);
        }
    }
}
