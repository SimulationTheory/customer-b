using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PSE.Customer.V1.Models
{
    public class LookupCustomer
    {
        [Required]
        public string NameOnBill { get; set; }
        [Required]
        public string ContractAccountNumber { get; set; }
    }
}
