using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PSE.Customer.V1.Models
{
    public class LookupCustomerRequest
    {
        [Required]
        public string NameOnBill { get; set; }
        [Required]
        public long ContractAccountNumber { get; set; }
    }
}
