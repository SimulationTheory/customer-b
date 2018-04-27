using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PSE.Customer.V1.Clients.Mcf.Request
{


    /// <summary>
    /// Request model for Cancel Move In requests to MCF.
    /// </summary>
    public class CancelMoveInRequest
    {
        /// <summary>
        /// Gets or sets the Contract ID to cancel.
        /// </summary>
        [JsonProperty("ContractID")]
        [Required]
        [MinLength(1)]
        public string ContractId { get; set; }
    }
}
