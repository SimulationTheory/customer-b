using System.Collections.Generic;
using Newtonsoft.Json;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Collection of all installments for a specific installment plan
    /// </summary>
    public class InstallmentPlansModel
    {
        /// <summary>
        /// Code for arrangement type, such as "005"
        /// </summary>
        /// <remarks>
        /// This comes from the SAP calls, but is not stored in Cassandra and would be null or blank
        /// </remarks>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        public string ArrangementType { get; set; }

        /// <summary>
        /// Collection of all installments, each consisting of a due date and amount due
        /// </summary>
        public IEnumerable<InstallmentDetails> Installments { get; set; }
    }
}
