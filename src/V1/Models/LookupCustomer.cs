using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PSE.Customer.V1.Models
{
    public class LookupCustomer: IValidatableObject
    {
        [Required]
        public string NameOnBill { get; set; }
        [Required]
        public string ContractAccountNumber { get; set; }        
        public string TaxId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (string.IsNullOrEmpty(ContractAccountNumber) && string.IsNullOrEmpty(TaxId))
                results.Add(new ValidationResult("At least one object is null or empty"));
            return results;
        }
    }
}
