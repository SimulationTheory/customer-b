namespace PSE.Customer.V1.Models
{
    public class ReconnectStatusResponse
    {
        public bool? IsEligibile { get; set; }
        public decimal AmountPosted { get; set; }
        public decimal MinimumPaymentRequired { get; set; }
        public decimal AmountLeftover { get; set; }
        public decimal Deposit { get; set; }

        public decimal FirstLp { get; set; }

        public decimal ReconnectAmount { get; set; }

        public long ContractAccountId { get; set; }
        public long PriorObligationContractAccountId { get; set; }
        public string BpId { get; set; }

        public bool? Reconnect { get; set; }
        public string AccountType { get; set; }
        public string Reason { get; set; }

        public string ReasonCode { get; set; }

    }
}
