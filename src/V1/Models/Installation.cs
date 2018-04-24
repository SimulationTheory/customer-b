using System;

namespace PSE.Customer.V1.Models
{
    /// <summary>
    /// A premise installation for the GetPremiseInstallationResponse object.
    /// </summary>
    public class Installation
    {
        public long InstallationId { get; set; }
        public bool MoveInEligibility { get; set; }
        public string DivisionId { get; set; }
        public long PremiseId { get; set; }
        public string InstallationGuid { get; set; }
        public long? BusinessPartnerId { get; set; }
        public string CurrentOwner { get; set; }
        public long? OwnerContractAccountId { get; set; }
        public DateTimeOffset? MoveInDateFrom { get; set; }
        public DateTimeOffset? MoveInDateTo { get; set; }
        public DateTimeOffset? FutureMoveInDate { get; set; }
        public string Fraud { get; set; }
        public string StreetLight { get; set; }
        public string Lease { get; set; }
        public decimal? LeaseAmount { get; set; }
        public string OrderPrinted { get; set; }
        public string LeaseType { get; set; }
        public string ProcessVariant { get; set; }
        public DateTimeOffset? LastBillEndDate { get; set; }
        public string DisconnectStatus { get; set; }
        public string DisconnectSource { get; set; }
        public DateTimeOffset? DisconnectDate { get; set; }
        /// <summary>
        /// The difference in days between today and DisconnectDate.
        /// </summary>
        public int? DisconnectDays { get; set; }
    }
}
