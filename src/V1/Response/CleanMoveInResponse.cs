using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Response
{
    public class CleanMoveInResponse
    {
        public IDictionary<string, string> DepositsByContractId { get; set; } 
        public IDictionary<string, string> NotificationNumberByContractId { get; set; }
    }
}
