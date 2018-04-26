using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Response
{
    public class LateMoveInResponse
    {
        public IEnumerable<long> NewContractAccounts { get; set; }
    }
}
