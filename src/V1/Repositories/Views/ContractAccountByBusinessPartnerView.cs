using Cassandra.Mapping.Attributes;

namespace PSE.Customer.V1.Repositories.Views
{
    [Table("bp_by_contract_account")]
    public class ContractAccountByBusinessPartnerView
    {
        [Column("contract_account_id")]
        public long ContractAccountId { get; set; }
        [Column("bp_id")]
        public long BusinessPartnerId { get; set; }
    }
}