using Cassandra.Mapping.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Repositories.Entities
{
    [Table("bp_by_contract_account")]
    public class BPByContractAccountEntity
    {
        [PartitionKey]
        [Column("contract_account_id")]
        public long ContractAccountId { get; set; }
        [ClusteringKey(0, Name = "bp_id")]
        public long BusinessPartner_Id { get; set; }
        [Column("bp_role")]
        public string BusinessPartner_Role { get; set; }
        [Column("bp_type")]
        public string BusinessPartner_Type { get; set; }
        [Column("authorized_time")]
        public DateTimeOffset AuthorizedTime { get; set; }
        [Column("revoked_time")]
        public DateTimeOffset RevokedTime { get; set; }
    }
}
