using System;

namespace PSE.Customer.V1.Clients.Device.Models
{
    /// <summary>
    /// An installation for the GetPremiseInstallationResponse object.
    /// </summary>
    public class Installation
    {
        /// <summary>
        /// Gets or sets the installation identifier.
        /// </summary>
        /// <value>
        /// The installation identifier.
        /// </value>
        public long InstallationId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [move in eligibility].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [move in eligibility]; otherwise, <c>false</c>.
        /// </value>
        public bool MoveInEligibility { get; set; }
        /// <summary>
        /// Gets or sets the division identifier.
        /// </summary>
        /// <value>
        /// The division identifier.
        /// </value>
        public string DivisionId { get; set; }
        /// <summary>
        /// Gets or sets the premise identifier.
        /// </summary>
        /// <value>
        /// The premise identifier.
        /// </value>
        public long PremiseId { get; set; }
        /// <summary>
        /// Gets or sets the installation unique identifier.
        /// </summary>
        /// <value>
        /// The installation unique identifier.
        /// </value>
        public string InstallationGuid { get; set; }
        /// <summary>
        /// Gets or sets the business partner identifier.
        /// </summary>
        /// <value>
        /// The business partner identifier.
        /// </value>
        public long? BusinessPartnerId { get; set; }
        /// <summary>
        /// Gets or sets the current owner.
        /// </summary>
        /// <value>
        /// The current owner.
        /// </value>
        public string CurrentOwner { get; set; }
        /// <summary>
        /// Gets or sets the owner contract account identifier.
        /// </summary>
        /// <value>
        /// The owner contract account identifier.
        /// </value>
        public long? OwnerContractAccountId { get; set; }
        /// <summary>
        /// Gets or sets the move in date from.
        /// </summary>
        /// <value>
        /// The move in date from.
        /// </value>
        public DateTimeOffset? MoveInDateFrom { get; set; }
        /// <summary>
        /// Gets or sets the move in date to.
        /// </summary>
        /// <value>
        /// The move in date to.
        /// </value>
        public DateTimeOffset? MoveInDateTo { get; set; }
        /// <summary>
        /// Gets or sets the future move in date.
        /// </summary>
        /// <value>
        /// The future move in date.
        /// </value>
        public DateTimeOffset? FutureMoveInDate { get; set; }
        /// <summary>
        /// Gets or sets the fraud.
        /// </summary>
        /// <value>
        /// The fraud.
        /// </value>
        public string Fraud { get; set; }
        /// <summary>
        /// Gets or sets the street light.
        /// </summary>
        /// <value>
        /// The street light.
        /// </value>
        public string StreetLight { get; set; }
        /// <summary>
        /// Gets or sets the lease.
        /// </summary>
        /// <value>
        /// The lease.
        /// </value>
        public string Lease { get; set; }
        /// <summary>
        /// Gets or sets the lease amount.
        /// </summary>
        /// <value>
        /// The lease amount.
        /// </value>
        public decimal? LeaseAmount { get; set; }
        /// <summary>
        /// Gets or sets the order printed.
        /// </summary>
        /// <value>
        /// The order printed.
        /// </value>
        public string OrderPrinted { get; set; }
        /// <summary>
        /// Gets or sets the type of the lease.
        /// </summary>
        /// <value>
        /// The type of the lease.
        /// </value>
        public string LeaseType { get; set; }
        /// <summary>
        /// Gets or sets the process variant.
        /// </summary>
        /// <value>
        /// The process variant.
        /// </value>
        public string ProcessVariant { get; set; }
        /// <summary>
        /// Gets or sets the last bill end date.
        /// </summary>
        /// <value>
        /// The last bill end date.
        /// </value>
        public DateTimeOffset? LastBillEndDate { get; set; }
        /// <summary>
        /// Gets or sets the disconnect status.
        /// </summary>
        /// <value>
        /// The disconnect status.
        /// </value>
        public string DisconnectStatus { get; set; }
        /// <summary>
        /// Gets or sets the disconnect source.
        /// </summary>
        /// <value>
        /// The disconnect source.
        /// </value>
        public string DisconnectSource { get; set; }
        /// <summary>
        /// Gets or sets the disconnect date.
        /// </summary>
        /// <value>
        /// The disconnect date.
        /// </value>
        public DateTimeOffset? DisconnectDate { get; set; }
        /// <summary>
        /// The difference in days between today and DisconnectDate.
        /// </summary>
        public int? DisconnectDays { get; set; }
    }
}
