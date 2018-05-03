using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Models
{
    public class OwnerPremiseSet
    {
        public OwnerPremiseAddress PremiseAddress { get; set; }

        public string Premise { get; set; }

        public McfList<OwnerPremisePropertySet> OwnerPremiseProperty { get; set; }
    }
}
