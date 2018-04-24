using System.Collections.Generic;

namespace PSE.Customer.V1.Models
{
    public class BpSearchModel
    {
        public long BpId { get; set; }

        public List<IdentifierModel> BpSearchIdentifiers { get; set; }

        public string Reason { get; set; }

        public string ReasonCode { get; set; }

        public bool MatchFound { get; set; }

        public BpSearchModel()
        {
            BpSearchIdentifiers = new List<IdentifierModel>();
        }
    }
}
