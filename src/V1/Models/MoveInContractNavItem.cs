using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Models
{
    public class MoveInContractNavItem
    {
        public McfMetadata Metadata { get; set; }
        public DateTimeOffset? AllowMIDteFrom { get; set; }
        public string TransferCA { get; set; }
        public DateTimeOffset? AllowMODteFrom { get; set; }
        [JsonProperty("Sdreason")]
        public string SDReason { get; set; }
        public string NotifLongTxt { get; set; }
        public string Streetlight { get; set; }
        public DateTimeOffset? AllowMODteTo { get; set; }
        public string NotifShortTxt { get; set; }
        public string Lease { get; set; }
        public string SDWaiverReason { get; set; }
        public string AllowCancelMO { get; set; }
        public string CustomerRole { get; set; }
        public decimal Leaseamt { get; set; }
        public string ContEndReason { get; set; }
        public string LeaseEquiNum { get; set; }
        public decimal SecDepositAmt { get; set; }
        public string ContractID { get; set; }
        public DateTimeOffset? AllowMIDteTo { get; set; }
        public string Description { get; set; }
        public string ProcessType { get; set; }
        public string ContractItemGUID { get; set; }
        public string Channel { get; set; }
        public string ContractHeaderGUID { get; set; }
        public DateTimeOffset? ContractStartDate { get; set; }
        public DateTimeOffset? ContractEndDate { get; set; }
        public string AccountID { get; set; }
        public string BusinessAgreementID { get; set; }
        public string ProductID { get; set; }
        public string DivisionID { get; set; }
        public string PointOfDeliveryGUID { get; set; }
        public string PremiseID { get; set; }
        public string FormerServiceProviderID { get; set; }
        public string NewServiceProviderID { get; set; }
        public string Distributor { get; set; }
        public string SwitchServiceFlag { get; set; }
        public string MeterNumber { get; set; }
        public bool HistoricFlag { get; set; }
    }
}
