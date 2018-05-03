using Newtonsoft.Json;
using PSE.WebAPI.Core.Interfaces;
using System;

namespace PSE.Customer.V1.Clients.Account.Models.Response
{
    public class GetContractItemResponse : IAPIResponse 
    {
        /// <summary>
        /// Gets or sets the allow mi DTE from.
        /// </summary>
        /// <value>
        /// The allow mi DTE from.
        /// </value>
        public DateTimeOffset? AllowMoveinDateFrom { get; set; }

        /// <summary>
        /// Gets or sets the transfer ca.
        /// </summary>
        /// <value>
        /// The transfer ca.
        /// </value>
        public string TransferCa { get; set; }

        /// <summary>
        /// Gets or sets the allow mo DTE from.
        /// </summary>
        /// <value>
        /// The allow mo DTE from.
        /// </value>
        public DateTimeOffset? AllowMoveoutDateFrom { get; set; }

        /// <summary>
        /// Gets or sets the sd reason.
        /// </summary>
        /// <value>
        /// The sd reason.
        /// </value>
        public string SdReason { get; set; }

        /// <summary>
        /// Gets or sets the notif long text.
        /// </summary>
        /// <value>
        /// The notif long text.
        /// </value>
        public string NotifyLongTxt { get; set; }

        /// <summary>
        /// Gets or sets the streetlight.
        /// </summary>
        /// <value>
        /// The streetlight.
        /// </value>
        public string Streetlight { get; set; }

        /// <summary>
        /// Gets or sets the allow moveout date to.
        /// </summary>
        /// <value>
        /// The allow mo DTE to.
        /// </value>
        public DateTimeOffset? AllowMoveoutDateTo { get; set; }

        /// <summary>
        /// Gets or sets the notif short text.
        /// </summary>
        /// <value>
        /// The notif short text.
        /// </value>
        public string NotifyShortTxt { get; set; }

        /// <summary>
        /// Gets or sets the lease.
        /// </summary>
        /// <value>
        /// The lease.
        /// </value>
        public string Lease { get; set; }

        /// <summary>
        /// Gets or sets the sd waiver reason.
        /// </summary>
        /// <value>
        /// The sd waiver reason.
        /// </value>
        public string SdWaiverReason { get; set; }

        /// <summary>
        /// Gets or sets the allow cancel moveout flag.
        /// </summary>
        /// <value>
        /// The allow cancel mo.
        /// </value>
        public string AllowCancelMoveout { get; set; }

        /// <summary>
        /// Gets or sets the customer role.
        /// </summary>
        /// <value>
        /// The customer role.
        /// </value>
        public string CustomerRole { get; set; }

        /// <summary>
        /// Gets or sets the lease amount.
        /// </summary>
        /// <value>
        /// The leaseamt.
        /// </value>
        public decimal LeaseAmount { get; set; }

        /// <summary>
        /// Gets or sets the cont end reason.
        /// </summary>
        /// <value>
        /// The cont end reason.
        /// </value>
        public string ContEndReason { get; set; }

        /// <summary>
        /// Gets or sets the lease equi number.
        /// </summary>
        /// <value>
        /// The lease equi number.
        /// </value>
        public string LeaseEquiNum { get; set; }

        /// <summary>
        /// Gets or sets the security deposit amt.
        /// </summary>
        /// <value>
        /// The sec deposit amt.
        /// </value>
        public decimal SecurityDepositAmount { get; set; }

        /// <summary>
        /// Gets or sets the contract identifier.
        /// </summary>
        /// <value>
        /// The contract identifier.
        /// </value>
        public long ContractId { get; set; }

        /// <summary>
        /// Gets or sets allow movein date to.
        /// </summary>
        /// <value>
        /// The allow mi DTE to.
        /// </value>
        public DateTimeOffset? AllowMoveinDateTo { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of the process.
        /// </summary>
        /// <value>
        /// The type of the process.
        /// </value>
        public string ProcessType { get; set; }

        /// <summary>
        /// Gets or sets the contract item unique identifier.
        /// </summary>
        /// <value>
        /// The contract item unique identifier.
        /// </value>
        public string ContractItemGuid { get; set; }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        public string Channel { get; set; }

        /// <summary>
        /// Gets or sets the contract header unique identifier.
        /// </summary>
        /// <value>
        /// The contract header unique identifier.
        /// </value>
        public string ContractHeaderGuid { get; set; }

        /// <summary>
        /// Gets or sets the contract start date.
        /// </summary>
        /// <value>
        /// The contract start date.
        /// </value>
        [JsonProperty("ContractStartDate")]
        public DateTimeOffset? ContractStartDate { get; set; }

        /// <summary>
        /// Gets or sets the contract end date.
        /// </summary>
        /// <value>
        /// The contract end date.
        /// </value>
        public DateTimeOffset? ContractEndDate { get; set; }

        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>
        /// The account identifier.
        /// </value>
        public long BusinessPartnerId { get; set; }

        /// <summary>
        /// Gets or sets the business agreement identifier.
        /// </summary>
        /// <value>
        /// The business agreement identifier.
        /// </value>
        public string ContractAccountId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the division identifier.
        /// </summary>
        /// <value>
        /// The division identifier.
        /// </value>
        public string DivisionId { get; set; }

        /// <summary>
        /// Gets or sets the point of delivery unique identifier.
        /// </summary>
        /// <value>
        /// The point of delivery unique identifier.
        /// </value>
        public string PointOfDeliveryGuid { get; set; }

        /// <summary>
        /// Gets or sets the premise identifier.
        /// </summary>
        /// <value>
        /// The premise identifier.
        /// </value>
        public long PremiseId { get; set; }

        /// <summary>
        /// Gets or sets the former service provider identifier.
        /// </summary>
        /// <value>
        /// The former service provider identifier.
        /// </value>
        public string FormerServiceProviderId { get; set; }

        /// <summary>
        /// Gets or sets the new service provider identifier.
        /// </summary>
        /// <value>
        /// The new service provider identifier.
        /// </value>
        public string NewServiceProviderId { get; set; }

        /// <summary>
        /// Gets or sets the distributor.
        /// </summary>
        /// <value>
        /// The distributor.
        /// </value>
        public string Distributor { get; set; }

        /// <summary>
        /// Gets or sets the switch service flag.
        /// </summary>
        /// <value>
        /// The switch service flag.
        /// </value>
        public string SwitchServiceFlag { get; set; }

        /// <summary>
        /// Gets or sets the meter number.
        /// </summary>
        /// <value>
        /// The meter number.
        /// </value>
        public string MeterNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [historic flag].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [historic flag]; otherwise, <c>false</c>.
        /// </value>
        public bool HistoricFlag { get; set; }

    }
}
