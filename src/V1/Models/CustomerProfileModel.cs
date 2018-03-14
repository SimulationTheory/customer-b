using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PSE.Customer.V1.Repositories.DefinedTypes;
using System.Collections.Generic;

namespace PSE.Customer.V1.Models
{
    public class CustomerProfileModel
    {        
        public string CustomerName { get; set; }
        public string OrganizationName { get; set; }
        public string IsPva { get; set; }
        public string EmailAddress { get; set; }        
        public AddressDefinedType MailingAddress { get; set; }
        public IEnumerable<Phone> Phones { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PhoneType? PrimaryPhone { get; set; }
    }
}
