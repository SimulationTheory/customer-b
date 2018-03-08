using System.Collections.Generic;

namespace PSE.Customer.V1.Models
{
    public class WebProfile
    {
        public string BPId { get; set; }
        public LookupCustomer Customer { get; set; }        
        public IEnumerable<SecurityQuestionResponse> SecurityQuestionResponses { get;set;}
        public CustomerCredentials CustomerCredentials { get; set; }
        public Phone Phone { get; set; }
        public bool TermsAndConditions { get; set; } 
    }
}
