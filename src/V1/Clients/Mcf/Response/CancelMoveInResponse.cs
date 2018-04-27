using Newtonsoft.Json;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Model for Cancel Move In MCF responses.
    /// </summary>
    public class CancelMoveInResponse : IMcfResult
    {
        /// <inheritdoc />
        public McfMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether move in has been cancelled.
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Gets or sets a message associated with the success flag. 
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Gets or sets the contract id.
        /// </summary>
        [JsonProperty("ContractID")]
        public string ContractId { get; set; }
    }
}
