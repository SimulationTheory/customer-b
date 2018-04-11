using Cassandra.Mapping.Attributes;
using PSE.Customer.V1.Repositories.DefinedTypes;
using System.Collections.Generic;

namespace PSE.Customer.V1.Repositories.Entities
{
    [Table("customer_contact")]
    public class CustomerContactEntity
    {
        [PartitionKey]
        [Column("bp_id")]
        public long BpId { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("mailing_address")]
        public AddressDefinedType MailingAddress { get; set; }
        [Column("phones")]
        public Dictionary<string, PhoneDefinedType> Phones { get; set; }
    }
}
