﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PSE.Customer.V1.Models
{
    public class WebProfile
    {
        [Required]
        public string BPId { get; set; }
        public LookupCustomerRequest Customer { get; set; }        
        public IEnumerable<SecurityQuestionResponse> SecurityQuestionResponses { get;set;}        
        public CustomerCredentials CustomerCredentials { get; set; }
        public Phone Phone { get; set; }

        public string Email { get; set; }

    }
}
