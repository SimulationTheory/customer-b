using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace PSE.McfClient.Tests.Unit
{
    [TestClass]
    public class McfResponseTest
    {
        IPAddress _ipAddress = new IPAddress(new byte[] { 1, 2, 3, 4 });
        [TestMethod]
        public void McfResponseTest_Constructor_2_Parameters_Must_Persist_Values()
        {
            // Arrange
            var httpStatusCode = HttpStatusCode.OK;

            // Act

            var result = new McfResponse<IPAddress>(_ipAddress, httpStatusCode);
            // Assert

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(httpStatusCode);
            result.Value.ShouldBe(_ipAddress);
            result.Message.ShouldBe(default(string));
        }

        [TestMethod]
        public void McfResponseTest_Constructor__Parameters_Must_Persist_Values()
        {
            // Arrange
            var httpStatusCode = HttpStatusCode.OK;
            var message = "message";
            // Act

            var result = new McfResponse<IPAddress>(_ipAddress, httpStatusCode, message);
            // Assert

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(httpStatusCode);
            result.Value.ShouldBe(_ipAddress);
            result.Message.ShouldBe(message);
        }

        [TestMethod]
        public void McfResponseTest_IsSuccessfulCode_Test()
        {
            // Arrange
            var resp = new HttpResponseMessage();
            foreach (var httpStatusCode in Enum.GetValues(typeof(HttpStatusCode)).Cast<HttpStatusCode>())
            {
                resp.StatusCode = httpStatusCode;
                var isConsideredSuccess = resp.IsSuccessStatusCode;

                // Act
                var result = new McfResponse<IPAddress>(_ipAddress, httpStatusCode);

                // Assert
                result.IsSuccessStatusCode.ShouldBe(isConsideredSuccess);
            }
        }
    }
}
