using PSE.Customer.V1.Models;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Response
{
    public class MoveInResponse : MoveInContractItemNav
    {
        public McfList<MoveInContractItemNav> ContractItemNav { get; set; }
    }
}
