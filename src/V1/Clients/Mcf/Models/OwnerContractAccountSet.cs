using Newtonsoft.Json;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Models
{
    public class OwnerContractAccountSet
    {
        public string ContractAccount { get; set; }

        [JsonProperty("ContractAccBal")]
        public string ContractAccountBalance { get; set; }

        public McfList<OwnerPremiseSet> OwnerPremise { get; set; }
    }
}
