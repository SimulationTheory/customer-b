using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Models
{
    public class ReconnectStatusResponse
    {
        public bool? IsEligibile { get; set; }
        public decimal AmountPosted { get; set; }
        public decimal MinimumPaymentRequired { get; set; }
        public decimal AmountLeftover { get; set; }
    }
}
