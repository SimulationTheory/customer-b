using Newtonsoft.Json.Converters;

namespace PSE.Customer.V1.Models
{
    public class Phone
    {   
        public PhoneType Type { get; set; }
        public string Number { get; set; }
        public string Extension { get; set; }
    }
}
