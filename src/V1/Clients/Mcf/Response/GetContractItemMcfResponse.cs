using Newtonsoft.Json;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;
using System;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Mcf contract item entity.
    /// </summary>
    /// <seealso cref="PSE.RestUtility.Core.Interfaces.IMcfResult" />
    public class GetContractItemMcfResponse : IMcfResult
    {
        /// <summary>
        /// Gets or sets the allow mi DTE from.
        /// </summary>
        /// <value>
        /// The allow mi DTE from.
        /// </value>
        [JsonProperty("AllowMIDteFrom")]
        public DateTimeOffset? AllowMoveinDateFrom { get; set; }

        /// <summary>
        /// Gets or sets the transfer ca.
        /// </summary>
        /// <value>
        /// The transfer ca.
        /// </value>
        [JsonProperty("TransferCA")]
        public string TransferCa { get; set; }

        /// <summary>
        /// Gets or sets the allow mo DTE from.
        /// </summary>
        /// <value>
        /// The allow mo DTE from.
        /// </value>
        [JsonProperty("AllowMODteFrom")]
        public DateTimeOffset? AllowMoveoutDateFrom { get; set; }

        /// <summary>
        /// Gets or sets the sd reason.
        /// </summary>
        /// <value>
        /// The sd reason.
        /// </value>
        [JsonProperty("SDReason")]
        public string SdReason { get; set; }

        /// <summary>
        /// Gets or sets the notif long text.
        /// </summary>
        /// <value>
        /// The notif long text.
        /// </value>
        [JsonProperty("NotifLongTxt")]
        public string NotifyLongTxt { get; set; }

        /// <summary>
        /// Gets or sets the streetlight.
        /// </summary>
        /// <value>
        /// The streetlight.
        /// </value>
        [JsonProperty("Streetlight")]
        public string Streetlight { get; set; }

        /// <summary>
        /// Gets or sets the allow moveout date to.
        /// </summary>
        /// <value>
        /// The allow mo DTE to.
        /// </value>
        [JsonProperty("AllowMODteTo")]
        public DateTimeOffset? AllowMoveoutDateTo { get; set; }

        /// <summary>
        /// Gets or sets the notif short text.
        /// </summary>
        /// <value>
        /// The notif short text.
        /// </value>
        [JsonProperty("NotifShortTxt")]
        public string NotifyShortTxt { get; set; }

        /// <summary>
        /// Gets or sets the lease.
        /// </summary>
        /// <value>
        /// The lease.
        /// </value>
        [JsonProperty("Lease")]
        public string Lease { get; set; }

        /// <summary>
        /// Gets or sets the sd waiver reason.
        /// </summary>
        /// <value>
        /// The sd waiver reason.
        /// </value>
        [JsonProperty("SDWaiverReason")]
        public string SdWaiverReason { get; set; }

        /// <summary>
        /// Gets or sets the allow cancel moveout flag.
        /// </summary>
        /// <value>
        /// The allow cancel mo.
        /// </value>
        [JsonProperty("AllowCancelMO")]
        public string AllowCancelMoveout { get; set; }

        /// <summary>
        /// Gets or sets the customer role.
        /// </summary>
        /// <value>
        /// The customer role.
        /// </value>
        [JsonProperty("CustomerRole")]
        public string CustomerRole { get; set; }

        /// <summary>
        /// Gets or sets the lease amount.
        /// </summary>
        /// <value>
        /// The leaseamt.
        /// </value>
        [JsonProperty("Leaseamt")]
        public decimal LeaseAmount { get; set; }

        /// <summary>
        /// Gets or sets the cont end reason.
        /// </summary>
        /// <value>
        /// The cont end reason.
        /// </value>
        [JsonProperty("ContEndReason")]
        public string ContEndReason { get; set; }

        /// <summary>
        /// Gets or sets the lease equi number.
        /// </summary>
        /// <value>
        /// The lease equi number.
        /// </value>
        [JsonProperty("LeaseEquiNum")]
        public string LeaseEquiNum { get; set; }

        /// <summary>
        /// Gets or sets the security deposit amt.
        /// </summary>
        /// <value>
        /// The sec deposit amt.
        /// </value>
        [JsonProperty("SecDepositAmt")]
        public decimal SecurityDepositAmount { get; set; }

        /// <summary>
        /// Gets or sets the contract identifier.
        /// </summary>
        /// <value>
        /// The contract identifier.
        /// </value>
        [JsonProperty("ContractID")]
        public long ContractId { get; set; }

        /// <summary>
        /// Gets or sets allow movein date to.
        /// </summary>
        /// <value>
        /// The allow mi DTE to.
        /// </value>
        [JsonProperty("AllowMIDteTo")]
        public DateTimeOffset? AllowMoveinDateTo { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [JsonProperty("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of the process.
        /// </summary>
        /// <value>
        /// The type of the process.
        /// </value>
        [JsonProperty("ProcessType")]
        public string ProcessType { get; set; }

        /// <summary>
        /// Gets or sets the contract item unique identifier.
        /// </summary>
        /// <value>
        /// The contract item unique identifier.
        /// </value>
        [JsonProperty("ContractItemGUID")]
        public string ContractItemGuid { get; set; }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        [JsonProperty("Channel")]
        public string Channel { get; set; }

        /// <summary>
        /// Gets or sets the contract header unique identifier.
        /// </summary>
        /// <value>
        /// The contract header unique identifier.
        /// </value>
        [JsonProperty("ContractHeaderGUID")]
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
        [JsonProperty("ContractEndDate")]
        public DateTimeOffset? ContractEndDate { get; set; }

        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>
        /// The account identifier.
        /// </value>
        [JsonProperty("AccountID")]
        public long BusinessPartnerId { get; set; }

        /// <summary>
        /// Gets or sets the business agreement identifier.
        /// </summary>
        /// <value>
        /// The business agreement identifier.
        /// </value>
        [JsonProperty("BusinessAgreementID")]
        public string ContractAccountId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        [JsonProperty("ProductID")]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the division identifier.
        /// </summary>
        /// <value>
        /// The division identifier.
        /// </value>
        [JsonProperty("DivisionID")]
        public string DivisionId { get; set; }

        /// <summary>
        /// Gets or sets the point of delivery unique identifier.
        /// </summary>
        /// <value>
        /// The point of delivery unique identifier.
        /// </value>
        [JsonProperty("PointOfDeliveryGUID")]
        public string PointOfDeliveryGuid { get; set; }

        /// <summary>
        /// Gets or sets the premise identifier.
        /// </summary>
        /// <value>
        /// The premise identifier.
        /// </value>
        [JsonProperty("PremiseID")]
        public long PremiseId { get; set; }

        /// <summary>
        /// Gets or sets the former service provider identifier.
        /// </summary>
        /// <value>
        /// The former service provider identifier.
        /// </value>
        [JsonProperty("FormerServiceProviderID")]
        public string FormerServiceProviderId { get; set; }

        /// <summary>
        /// Gets or sets the new service provider identifier.
        /// </summary>
        /// <value>
        /// The new service provider identifier.
        /// </value>
        [JsonProperty("NewServiceProviderID")]
        public string NewServiceProviderId { get; set; }

        /// <summary>
        /// Gets or sets the distributor.
        /// </summary>
        /// <value>
        /// The distributor.
        /// </value>
        [JsonProperty("Distributor")]
        public string Distributor { get; set; }

        /// <summary>
        /// Gets or sets the switch service flag.
        /// </summary>
        /// <value>
        /// The switch service flag.
        /// </value>
        [JsonProperty("SwitchServiceFlag")]
        public string SwitchServiceFlag { get; set; }

        /// <summary>
        /// Gets or sets the meter number.
        /// </summary>
        /// <value>
        /// The meter number.
        /// </value>
        [JsonProperty("MeterNumber")]
        public string MeterNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [historic flag].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [historic flag]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("HistoricFlag")]
        public bool HistoricFlag { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata is ignored
        /// </value>
        [JsonIgnore]
        public McfMetadata Metadata { get; set; }

        /// <summary>
        /// Gets the sample data.
        /// </summary>
        /// <returns></returns>
        public static string GetSampleData()
        {
            return "{\r\n    \"d\": {\r\n        \"__metadata\": {\r\n            \"id\": \"http://ssapngdn2apv1.puget.com:8002/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/ContractItems('4000001665')\",\r\n            \"uri\": \"http://ssapngdn2apv1.puget.com:8002/sap/opu/odata/sap/ZCRM_UTILITIES_UMC_PSE_SRV/ContractItems('4000001665')\",\r\n            \"type\": \"ZCRM_UTILITIES_UMC_PSE_SRV.ContractItem\"\r\n        },\r\n        \"AllowMIDteFrom\": null,\r\n        \"TransferCA\": \"\",\r\n        \"AllowMODteFrom\": null,\r\n        \"SDReason\": \"0001\",\r\n        \"NotifLongTxt\": \"\",\r\n        \"Streetlight\": \"\",\r\n        \"AllowMODteTo\": null,\r\n        \"NotifShortTxt\": \"\",\r\n        \"Lease\": \"\",\r\n        \"SDWaiverReason\": \"\",\r\n        \"AllowCancelMO\": \"\",\r\n        \"CustomerRole\": \"\",\r\n        \"Leaseamt\": \"0.000\",\r\n        \"ContEndReason\": \"\",\r\n        \"LeaseEquiNum\": \"\",\r\n        \"SecDepositAmt\": \"120.000\",\r\n        \"ContractID\": \"4000001665\",\r\n        \"AllowMIDteTo\": null,\r\n        \"Description\": \"RES Elec Service\",\r\n        \"ProcessType\": \"\",\r\n        \"ContractItemGUID\": \"005056a8-0123-1ed2-83b5-769950fbaaf1\",\r\n        \"Channel\": \"\",\r\n        \"ContractHeaderGUID\": \"502179df-cfbd-3030-e100-8000aac09746\",\r\n        \"ContractStartDate\": \"/Date(1349308800000)/\",\r\n        \"ContractEndDate\": \"/Date(253402214400000)/\",\r\n        \"AccountID\": \"1000002456\",\r\n        \"BusinessAgreementID\": \"200000036307\",\r\n        \"ProductID\": \"ERES_7E\",\r\n        \"DivisionID\": \"10\",\r\n        \"PointOfDeliveryGUID\": \"K252r{SjFv3X0800gi2NHW\",\r\n        \"PremiseID\": \"7000002897\",\r\n        \"FormerServiceProviderID\": \"\",\r\n        \"NewServiceProviderID\": \"\",\r\n        \"Distributor\": \"\",\r\n        \"SwitchServiceFlag\": \"\",\r\n        \"MeterNumber\": \"\",\r\n        \"HistoricFlag\": false\r\n    }\r\n}";
        }
    }
}
