using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PSE.Customer.V1.Models
{
    public class LookupCustomerRequest
    {
        /// <summary>
        /// Name on Bill. Full name, all caps. 
        /// </summary>
        [Required]
        public string NameOnBill { get; set; }
        /// <summary>
        /// Contract account number, must match name on bill. 
        /// </summary>
        [Required]
        public long ContractAccountNumber { get; set; }
    }
}
