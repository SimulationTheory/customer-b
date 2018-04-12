using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSE.Customer.Tests.Integration.TestObjects;
using PSE.Customer.V1.Models;
using PSE.Test.Core;
using PSE.WebAPI.Core.Configuration;
using Shouldly;
using TestUser = PSE.Test.Core.TestUser;

namespace PSE.Customer.Tests.Integration
{
    [TestClass]
    public class MoveInRestTests
    {
        public static readonly TestUser User = new TestUser
        {
            ContractAccountId = 200009468303,
            BPNumber = 1001694236,
            Username = "testuser1"
        };

        /// <summary>
        /// Initializes the Test Assembly.
        /// </summary>
        /// <param name="context">The context.</param>
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            RestTests<TestStartup>.LoadBalancerUrl = CoreHelper.GetConfiguration<WebConfiguration>().LoadBalancerUrl;
        }

        [TestClass]
        public class MoveInLatePayments : RestTests<TestStartup>
        {
            /// <summary>
            /// Tests the MoveInLatePayments() method. Golden path.
            /// </summary>
            [Ignore]
            [TestMethod]
            public async Task MoveInLatePayments_1_R2E_Test()
            {
                // init vars
                var version = "1.0";
                var contractAccountId = User.ContractAccountId;
                var path = $"/v{version}/customer/movein-status-latepayment/{contractAccountId}";
                var jwtToken = GetJwtToken(User.Username);

                // test target service
                using (var results = await Get(path, jwtToken))
                {
                    // assertions
                    var content = await results.Content.ReadAsStringAsync();
                    Assert.AreEqual(HttpStatusCode.OK, results.StatusCode);

                    var latePayments = content.To<MoveInLatePaymentsResponse>();
                    latePayments.FirstIp.ShouldBe(286.00m);
                    latePayments.EligibleRc.ShouldBe(true);
                    latePayments.AccountNo.ShouldBe(200028750178);
                    latePayments.ReconnectFlag.ShouldBe(false);
                    latePayments.PriorObligationAccount.ShouldBe(200026140646);
                    latePayments.DepositAmount.ShouldBe(572.00m);
                    latePayments.ReconAmount.ShouldBe(70.00m);
                    latePayments.MinPayment.ShouldBe(356);
                    latePayments.IncPayment.ShouldBe(320.00m);
                    latePayments.AccountType.ShouldBe("RES");
                    latePayments.ReasonCode.ShouldBe(string.Empty);
                    latePayments.Reason.ShouldBe(string.Empty);
                }
            }

            /// <summary>
            /// Tests the MoveInLatePayments() method over a rest call. 
            /// ContractAccountValidator enabled, so bp/contractAccountId must match. Golden path.
            /// </summary>
            [Ignore]
            [TestMethod]
            public async Task MoveInLatePayments_1_R2E_ContractAccountValidator_Test()
            {
                // init vars
                var version = "1.0";
                var contractAccountId = User.ContractAccountId;
                var path = $"/v{version}/customer/movein-status-latepayment/{contractAccountId}";
                var jwtToken = GetJwtToken(User.Username);

                // test target service
                using (var results = await Get(path, jwtToken))
                {
                    // assertions
                    var content = await results.Content.ReadAsStringAsync();
                    Assert.AreEqual(HttpStatusCode.OK, results.StatusCode);
                }
            }

            /// <summary>
            /// Tests the MoveInLatePayments() method over a rest call when the bp doesnt match the 
            /// contract account id. Detected by the ContractAccountValidator
            /// </summary>
            [TestMethod]
            [Ignore("Currently failing.  Need to get update for PSE.Test.Core.RestTests to fix this.")]
            public async Task MoveInLatePayments_1_R2E_ContractAccountValidator_MismatchForbidden_Test()
            {
                // init vars
                var version = "1.0";
                var contractAccountId = 1234567890;
                var path = $"/v{version}/customer/movein-status-latepayment/{contractAccountId}";
                var jwtToken = GetJwtToken(User.Username);

                // test target service
                using (var results = await Get(path, jwtToken))
                {
                    // assertions
                    var content = await results.Content.ReadAsStringAsync();
                    Assert.AreEqual(HttpStatusCode.Forbidden, results.StatusCode);
                }
            }

            /// <summary>
            /// Tests the MoveInLatePayments(reconnectFlag) method. Golden path.
            /// </summary>
            [TestMethod]
            [Ignore("The request requires a header value be set for request-channel.  Not sure how to pass this to the RestTests")]
            public async Task MoveInLatePayments_2_R2E_Test()
            {
                // init vars
                var version = "1.0";
                var contractAccountId = User.ContractAccountId;
                var path = $"/v{version}/customer/movein-latepayment/{contractAccountId}?reconnectFlag=true";
                var jwtToken = GetJwtToken(User.Username);

                // test target service
                using (var results = await Put(path, "", jwtToken))
                {
                    // assertions
                    var content = await results.Content.ReadAsStringAsync();
                    Assert.AreEqual(HttpStatusCode.OK, results.StatusCode);

                    var latePayments = content.To<MoveInLatePaymentsResponse>();
                    latePayments.FirstIp.ShouldBe(286.00m);
                    latePayments.EligibleRc.ShouldBe(null);
                    latePayments.AccountNo.ShouldBe(200028750178);
                    latePayments.ReconnectFlag.ShouldBe(false);
                    latePayments.PriorObligationAccount.ShouldBe(200026140646);
                    latePayments.DepositAmount.ShouldBe(572.00m);
                    latePayments.ReconAmount.ShouldBe(70.00m);
                    latePayments.MinPayment.ShouldBe(356);
                    latePayments.IncPayment.ShouldBe(320.00m);
                    latePayments.AccountType.ShouldBe("RES");
                    latePayments.ReasonCode.ShouldBe(string.Empty);
                    latePayments.Reason.ShouldBe(string.Empty);
                }
            }

            /// <summary>
            /// Tests the MoveInLatePayments(reconnectFlag) method over a rest call. 
            /// ContractAccountValidator enabled, so bp/contractAccountId must match. Golden path.
            /// </summary>
            [TestMethod]
            [Ignore("The request requires a header value be set for request-channel.  Not sure how to pass this to the RestTests")]
            public async Task MoveInLatePayments_2_R2E_ContractAccountValidator_Test()
            {
                // init vars
                var version = "1.0";
                var contractAccountId = User.ContractAccountId;
                var path = $"/v{version}/customer/movein-latepayment/{contractAccountId}?reconnectFlag=true";
                var jwtToken = GetJwtToken(User.Username);

                // test target service
                using (var results = await Put(path, "", jwtToken))
                {
                    // assertions
                    var content = await results.Content.ReadAsStringAsync();
                    Assert.AreEqual(HttpStatusCode.OK, results.StatusCode);
                }
            }

            /// <summary>
            /// Tests the MoveInLatePayments(reconnectFlag) method over a rest call when the bp doesnt match the 
            /// contract account id. Detected by the ContractAccountValidator
            /// </summary>
            [TestMethod]
            [Ignore("Currently failing.  Need to get update for PSE.Test.Core.RestTests to fix this.")]
            public async Task MoveInLatePayments_2_R2E_ContractAccountValidator_MismatchForbidden_Test()
            {
                // init vars
                var version = "1.0";
                var contractAccountId = 1234567890;
                var path = $"/v{version}/customer/movein-latepayment/{contractAccountId}?reconnectFlag=true";
                var jwtToken = GetJwtToken(User.Username);

                // test target service
                using (var results = await Put(path, "", jwtToken))
                {
                    // assertions
                    var content = await results.Content.ReadAsStringAsync();
                    Assert.AreEqual(HttpStatusCode.Forbidden, results.StatusCode);
                }
            }
        }
    }
}
