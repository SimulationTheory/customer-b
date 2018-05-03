using PSE.Customer.V1.Models;
using PSE.RestUtility.Core.Mcf;
using PSE.WebAPI.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace PSE.Customer.V1.Response
{
    /// <summary>
    /// Response to handle a moveout stop service request.
    /// </summary>
    /// <seealso cref="PSE.WebAPI.Core.Interfaces.IAPIResponse" />
    public class MoveOutStopServiceResponse : IAPIResponse
    {
        /// <summary>
        /// The final bill date.
        /// </summary>
        /// <value>
        /// The final bill date.
        /// </value>
        public DateTimeOffset? FinalBillDate { get; set; }

        /// <summary>
        /// The final bill due date.
        /// </summary>
        /// <value>
        /// The final bill due date.
        /// </value>
        public DateTimeOffset? FinalBillDueDate { get; set; }

        /// <summary>
        /// The warm home fund amount.
        /// </summary>
        /// <value>
        /// The warm home fund.
        /// </value>
        public decimal? WarmHomeFund { get; set; }

        /// <summary>
        /// Indicates success of the stop service call for each installation.
        /// </summary>
        public Dictionary<long, string> Status { get; set; }
    }
}
