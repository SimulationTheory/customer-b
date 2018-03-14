using PSE.Cassandra.Core.Session.Interfaces;
using System.Runtime.Serialization;

namespace PSE.Customer.V1.Repositories.DefinedTypes
{
    [DataContract(Name = "phone")]
    public class PhoneDefinedType : IUserDefinedType
    {
        public string Number { get; set; }
        public string Extension { get; set; }
    }
}
