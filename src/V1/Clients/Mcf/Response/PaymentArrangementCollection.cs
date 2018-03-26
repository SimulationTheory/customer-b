using System.Collections.Generic;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Wrapper collection for installment
    /// </summary>
    public class PaymentArrangementCollection
    {
        /// <summary>
        /// Gets or sets the results from the SAP query
        /// </summary>
        /// <value>
        /// Zero or more payment arrangements
        /// </value>
        public List<InstallmentDetails> Results { get; set; }
    }
}
