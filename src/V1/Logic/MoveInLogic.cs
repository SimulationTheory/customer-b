using PSE.Customer.V1.Logic.Interfaces;
using System;
using Microsoft.Extensions.Logging;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Models;

namespace PSE.Customer.V1.Logic
{
    public class MoveInLogic : IMoveInLogic
    {
        private readonly ILogger<MoveInLogic> _logger;
        private readonly IMcfClient _mcfClient;

        public MoveInLogic(ILogger<MoveInLogic> logger, IMcfClient mcfClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mcfClient = mcfClient;
        }


        /// <inheritdoc />
        public ReconnectStatusResponse GetMoveInLatePayment(long contractAccountId, string jwt)
        {
            _logger.LogInformation($"Getting elibigbility info: GetMoveInLatePaymentResponse({nameof(contractAccountId)} : {contractAccountId})");
            var paymentResponse = _mcfClient.GetMoveInLatePaymentsResponse(contractAccountId, jwt);

            var reconnectStatus = new ReconnectStatusResponse()
            {
                IsEligibile = paymentResponse.EligibleRc ?? false,
                AmountPosted = paymentResponse.IncPayment,
                MinimumPaymentRequired = paymentResponse.MinPayment,
                AmountLeftover = paymentResponse.MinPayment - paymentResponse.IncPayment
            };
            
            return reconnectStatus;
        }


        //public ReconnectionResponse GetReconnectAmountDueAndCa(string contractAccountId)
        //{

        //    var paymentResponse = _mcfClient.GetMoveInLatePaymentsResponse(contractAccountId);


        //}
    }
}
