using System.Collections.Generic;
using System.Data.Services.Client;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    public class PaymentArrangement
    {
        public string ContractAccountID { get; set; }

        public string InstallmentPlanNumber { get; set; }

        public string InstallmentPlanType { get; set; }

        public string NoOfPaymentsPermitted { get; set; }

        public string PaStatus { get; set; }

        public McfResultsSet<InstallmentPlans> PaymentArrangementNav { get; set; }
    }
}
