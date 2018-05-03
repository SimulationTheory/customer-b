using PSE.WebAPI.Core.Interfaces;
using System.Collections.Generic;

namespace PSE.Customer.V1.Clients.Account.Models.Response
{
    public class GetAccountDetailsResponse : IAPIResponse
    {
        public long ContractAccountId { get; set; }
        public decimal AmountDue { get; set; }
        public decimal PastDueAmount { get; set; }
        public string DueDate { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string DunningLevel { get; set; }
        public bool Ebill { get; set; }
        public bool BudgetBill { get; set; }
        public bool PaymentArragement { get; set; }
        public bool GreenSolar { get; set; }
        public bool Carbon { get; set; }
        public bool BadPay { get; set; }
        public bool Bankruptcy { get; set; }
        public bool FeeIndicator { get; set; }
        public string AccountDeterminationId { get; set; }
        public decimal WarmHomeFundAmount { get; set; }
        public IEnumerable<DeviceLocation> DeviceLocations { get; set; }
    }
}
