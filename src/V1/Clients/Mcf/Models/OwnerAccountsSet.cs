using Newtonsoft.Json;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Models
{

    /// <summary>
    /// Collection of owner contract accounts
    /// </summary>
    public class OwnerAccountsSet
    {
        [JsonProperty("Accountid")]
        public string AccountId { get; set; }

        public McfList<OwnerContractAccountSet> OwnerContractAccount { get; set; }
    }
}
