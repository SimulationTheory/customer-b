using System;
using Cassandra.Mapping.Attributes;
using PSE.Customer.V1.Repositories.DefinedTypes;

namespace PSE.Customer.V1.Repositories.Entities
{
    [Table("contract_account")]
    public class ContractAccountEntity
    {
        [PartitionKey]
        [Column("bp_id")]
        public long BusinessPartnerId { get; set; }
        [ClusteringKey(0, Name = "contract_account_id")]
        public long ContractAccountId { get; set; }
        [Column("active")]
        public bool Active { get; set; }
        [Column("aft_ind")]
        public bool AFTIndicator { get; set; }
        [Column("balance_forward")]
        public decimal BalanceForward { get; set; }
        [Column("bankruptcy_ind")]
        public bool BankruptcyIndicator { get; set; }
        [Column("bdhv_free_ind")]
        public bool BDHVFreeIndicator { get; set; }
        [Column("budget_plan_ind")]
        public bool BudgetPlanIndicator { get; set; }
        [Column("close_date")]
        public DateTime? CloseDate { get; set; }
        [Column("collective_ind")]
        public bool CollectiveIndicator { get; set; }
        [Column("customer_type")]
        public string CustomerType { get; set; }
        [Column("has_pva")]
        public bool HasPVA { get; set; }
        [Column("next_stmt_date")]
        public DateTime? NextStatementDate { get; set; }
        [Column("on_payment_arrangement")]
        public bool OnPaymentArrangement { get; set; }
        [Column("open_date")]
        public DateTime OpenDate { get; set; }
        [Column("parent_account")]
        public long ParentAccount { get; set; }
        [Column("past_due_amt")]
        public decimal PastDueAmount { get; set; }
        [Column("pay_amount")]
        public decimal PayAmount { get; set; }
        [Column("premise_id")]
        public long PremiseId { get; set; }
        [Column("refresh_time")]
        public DateTime? RefreshTime { get; set; }
        [Column("service_address")]
        public AddressDefinedType ServiceAddress { get; set; }
        [Column("statement_dist_method")]
        public string StatementDistributionMethod { get; set; }
        [Column("total_amount_due")]
        public decimal TotalAmountDue { get; set; }
    }
}