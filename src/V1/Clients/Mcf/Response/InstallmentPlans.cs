using System.Data.Services.Client;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    public class InstallmentPlans
    {
        public string InstallmentPlanNumber { get; set; }

        public string InstallmentPlanType { get; set; }

        public string NoOfInstallments { get; set; }

        public McfResultsSet<InstallmentDetails> InstallmentPlansNav { get; set; }
    }
}
