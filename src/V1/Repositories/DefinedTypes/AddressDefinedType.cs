using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using PSE.Cassandra.Core.Session.Interfaces;

namespace PSE.Customer.V1.Repositories.DefinedTypes
{
    [DataContract(Name = "address")]
    public class AddressDefinedType : IUserDefinedType
    {
        [DataMember(Name = "line_1")]
        [Required]
        public string AddressLine1 { get; set; }

        [DataMember(Name = "line_2")]
        public string AddressLine2 { get; set; }

        [DataMember]
        [Required]
        public string City { get; set; }

        [DataMember]
        [Required]
        public string State { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember(Name = "postal_code")]
        [Required]
        public string PostalCode { get; set; }
    }
}