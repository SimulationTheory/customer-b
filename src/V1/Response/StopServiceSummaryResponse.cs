using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Response
{
    /// <summary>
    /// Response with summary account info used in the Stop Service workflow.
    /// </summary>
    /// <seealso cref="PSE.WebAPI.Core.Interfaces.IAPIResponse" />
    public class StopServiceSummaryResponse : IAPIResponse
    {
        /// <summary>
        /// Gets or sets the security deposit amount.
        /// </summary>
        /// <value>
        /// The security deposit amount.
        /// </value>
        public decimal? SecurityDepositAmount { get; set; }
    }
}
