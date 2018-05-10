using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.Customer.V1.Models;

namespace PSE.Customer.V1.Request
{
    public class CleanMoveInRequest
    {
        public string ContractAccountId { get; set; }
        public string tenantBPId { get; set; }
        public IEnumerable<ProductInfo> ProductEnrollments { get; set; }
        public IEnumerable<Installation> Installations { get; set; }
        public string IncomeSource { get; set; }
        public string EmploymentLength { get; set; }
        //TODO: Add applicable preferences and implement preference updates
        public string PhoneNumberUpdate { get; set; }
        public string PhoneNumberType { get; set; }
        public string EmailUpdate { get; set; }
        public string MeterAccessNotes { get; set; }
        public string ServiceNotificationNumber { get; set; }
    }
}
