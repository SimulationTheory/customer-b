using Cassandra.Mapping.Attributes;
using System;

namespace PSE.Customer.V1.Repositories.Entities
{
    [Table("customer")]
    public class CustomerEntity
    {
        [PartitionKey]
        [Column("bp_id")]
        public long BusinessPartnerId { get; set; }
        [Column("employer_name")]
        public string EmployerName { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("full_name")]
        public string FullName { get; set; }
        [Column("middle_name")]
        public string MiddleName { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("fraud_check")]
        public bool FraudCheck { get; set; }
        [Column("pva_ind")]
        public bool PvaIndicator { get; set; }
        [Column("refresh_time")]
        public DateTimeOffset RefreshTime { get; set; }
    }
}
