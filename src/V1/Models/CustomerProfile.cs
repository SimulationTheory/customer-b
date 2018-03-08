using System.Collections.Generic;

namespace PSE.Customer.V1.Models
{
    public class CustomerProfile
    {        
        public string EmailAddress { get; set; }        
        public Address MailingAddress { get; set; }        
        public IEnumerable<Phone> Phones { get; set; }
        public PhoneType PrimaryPhone { get; set; }
        public CustomerCredentials CustomerCredentials { get; set; }
    }
}
