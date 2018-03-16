using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSE.Customer.V1.Clients.ClientProxy;
using Shouldly;

namespace PSE.Customer.Tests.Unit.V1.Clients
{
    [TestClass]
    public class ClientProxyTests
    {
        #region Constructor

        [TestMethod]
        public void Constructor_AppSettingsIsNull_ExceptionThrown()
        {
            // Arrange
            ClientProxy client = null;

            // Act
            Action action = () => client = new ClientProxy(null);

            // Assert
            client.ShouldBeNull();
            action.ShouldThrow<ArgumentNullException>().
                ParamName.ShouldBe("coreOptions");
        }

        #endregion
    }
}
