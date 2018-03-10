using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PSE.Customer.V1.Repositories.DefinedTypes;
using System.Collections.Generic;

namespace PSE.Customer.V1.Models
{
    public class CustomerProfile
    {        
        public string CustomerName { get; set; }
        public string IsPva { get; set; }
        public string EmailAddress { get; set; }        
        public AddressDefinedType MailingAddress { get; set; }        
        [JsonConverter(typeof(StringEnumConverter))]
        public IEnumerable<Phone> Phones { get; set; }
        public PhoneType PrimaryPhone { get; set; }
    }
}
