using System.Collections.Generic;

namespace PSE.Customer.V1.Request
{
    public class MoveInRequest
    {
        public long ContractAccountId { get; set; }
        public IEnumerable<long> InstallationIds { get; set; }
        public long PremiseId { get; set; }
    }
}
