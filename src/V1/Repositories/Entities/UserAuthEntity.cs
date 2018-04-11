using Cassandra.Mapping.Attributes;
using System;

namespace PSE.Customer.V1.Repositories.Entities
{
    [Table("user_auth")]
    public class UserAuthEntity
    {
        [PartitionKey]
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("cognito_id")]
        public Guid CognitoId { get; set; }
        [Column("bp_id")]
        public long BusinessPartnerId { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("failure_count")]
        public int FailureCount { get; set; }
        [Column("legacy_salt")]
        public string LegacySalt { get; set; }
        [Column("user_name")]
        public string UserName { get; set; }
        [Column("locked")]
        public bool Locked { get; set; }
        [Column("last_attempt")]
        public DateTimeOffset LastAttempt { get; set; }
        [Column("last_login")]
        public DateTimeOffset LastLogin { get; set; }
        [Column("lock_time")]
        public DateTimeOffset LockTime { get; set; }
    }
}
