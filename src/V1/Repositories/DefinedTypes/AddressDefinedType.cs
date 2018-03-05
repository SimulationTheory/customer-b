using System.Runtime.Serialization;
using PSE.Cassandra.Core.Session.Interfaces;

namespace PSE.Customer.V1.Repositories.DefinedTypes
{
    [DataContract(Name = "address")]
    public class AddressDefinedType : IUserDefinedType
    {
        [DataMember(Name = "line_1")]
        public string AddressLine1 { get; set; }

        [DataMember(Name = "line_2")]
        public string AddressLine2 { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember(Name = "postal_code")]
        public string PostalCode { get; set; }
    }
}