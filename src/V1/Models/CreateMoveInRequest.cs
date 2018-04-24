using System.Collections.Generic;

namespace PSE.Customer.V1.Models
{
    public class CreateMoveInRequest
    {
        public string AccountID { get; set; }
        public string CustomerRole { get; set; }
        public string ProcessType { get; set; }
        public IEnumerable<ContractItemNav> ContractItemNav { get; set; }
        public IEnumerable<ProdAttributes> ProdAttributes { get; set; }
    }
}
