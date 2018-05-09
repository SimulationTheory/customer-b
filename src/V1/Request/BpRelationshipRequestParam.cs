using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Request
{
    public class BpRelationshipRequestParam
    {
        public string LoggedInBp { get; set; }
        public string Jwt { get; set; }
        public string TenantBp { get; set; }
    }
}
