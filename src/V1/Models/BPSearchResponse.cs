using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Models
{
    public class BPSearchResponse : IAPIResponse
    {
        public string BPId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }
        public string OrganaizationName { get; set; }
        public string MatchPercent { get; set; }

        public string Reasoncode { get; set; }

        public string Reason { get; set; }
    }
}