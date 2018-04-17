using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Request
{
    public class CreateBusinesspartnerRequest : IAPIRequest
    {
      
        public string Channel { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PartnerCategoryType PartnerCategory { get; set; }
       
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string OrgName { get; set; }
       
        public string Email { get; set; }

        //[JsonConverter(typeof(StringEnumConverter))]
        public Phone Phone { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public McfAddressType AddressType { get; set; }
        public AddressDefinedType Address { get; set; }

    }
}