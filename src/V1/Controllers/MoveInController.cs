using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PSE.WebAPI.Core.Service;

namespace PSE.Customer.V1.Controllers
{
    public class MoveInLatePaymentsResponse
    {
        public decimal? FirstIp { get; set; }
        public bool? EligibleRc { get; set; }
        public long AccountNo { get; set; }
        public bool? ReconnectFlag { get; set; }
        public long? PriorObligationAccount { get; set; }
        public decimal? DepositAmount { get; set; }
        public decimal? ReconAmount { get; set; }
        public decimal? MinPayment { get; set; }
        public decimal? IncPayment { get; set; }
        public string AccountType { get; set; }
        public string ReasonCode { get; set; }
        public string Reason { get; set; }
    }

    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/customer/")]
    public class MoveInController : PSEController
    {
        /// <summary>
        /// Mock Endpoint for Latepayments movein 
        /// 
        /// A    "FirstIp" : "286.000", (first installment which is 50% of (B) ie total deposit ) 
        ///B    "DepositAmount" : "572.000", (total deposit amount) 
        ///C    "ReconAmount" : "70.000", (reconnection fees) 
        ///D    "MinPayment" : "356.000", (Item A + B)
        ///E    "IncPayment" : "320.000",  (Payment which have been made)
        ///F    "EligibleRc" : "", (If this is ‘X’ it means customer has made thresh-hold payment and s/he is eligible for reconnection)
        ///If ReconnectFlag eq 'X', it will initiate reconnection
        /// </summary>
        /// <param name="reconnectFlag">If ReconnectFlag eq 'X', it will initiate reconnection</param>
        /// <returns></returns>
        [HttpGet("movein-status-latepayment/{contractAccountId}")]
        [ProducesResponseType(typeof(MoveInLatePaymentsResponse), 200)]
        public async Task<IActionResult> MoveInLatePayments()
        {
            IActionResult result = Ok(new MoveInLatePaymentsResponse()
            {
                FirstIp = 286.00m,
                EligibleRc = true,
                AccountNo = 200028750178,
                ReconnectFlag = false,
                PriorObligationAccount = 200026140646,
                DepositAmount = 572.00m,
                ReconAmount = 70.00m,
                MinPayment = 356,
                IncPayment = 320.00m,
                AccountType = "RES",
                ReasonCode = string.Empty,
                Reason = string.Empty
            });

            return result;
        }

        /// <summary>
        /// Mock Endpoint for Latepayments movein Put, initiates movein and creates a new contract account for obligated account. 
        /// When user pays minimum/all outstanding balance, user move-in status shows "EligibleRc" to true.
        /// Call this method by setting reconnectFlag to activate the service.
        /// </summary>
        /// <param name="reconnectFlag"></param>
        /// <returns></returns>
        [HttpPut("movein-latepayment/{contractAccountId}")]
        [ProducesResponseType(typeof(MoveInLatePaymentsResponse), 200)]
        public async Task<IActionResult> MoveInLatePayments([FromQuery]bool reconnectFlag)
        {
            IActionResult result = Ok(new MoveInLatePaymentsResponse()
            {

                FirstIp = 286.00m,
                EligibleRc = null,
                AccountNo = 200028750178,
                ReconnectFlag = false,
                PriorObligationAccount = 200026140646,
                DepositAmount = 572.00m,
                ReconAmount = 70.00m,
                MinPayment = 356,
                IncPayment = 320.00m,
                AccountType = "RES",
                ReasonCode = string.Empty,
                Reason = string.Empty
            });

            return result;
        }
    }
}