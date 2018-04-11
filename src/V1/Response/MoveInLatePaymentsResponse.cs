namespace PSE.Customer.V1.Response
{
    public class MoveInLatePaymentsResponse
    {
        public decimal? FirstIp { get; set; }
        public bool? EligibleRc { get; set; }
        public long AccountNo { get; set; }
        public bool? ReconnectFlag { get; set; }
        public long? PriorObligationAccount { get; set; }
        public decimal? DepositAmount { get; set; }
        public decimal? ReconAmount { get; set; }
        public decimal? MinPayment { get; set; }
        public decimal? IncPayment { get; set; }
        public string AccountType { get; set; }
        public string ReasonCode { get; set; }
        public string Reason { get; set; }
    }
}
