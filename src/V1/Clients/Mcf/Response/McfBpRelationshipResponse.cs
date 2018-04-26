using PSE.Customer.V1.Request;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    public class McfBpRelationshipResponse 
    {
        public string AccountID1 { get; set; }
        public string AccountID2 { get; set; }

        public string Message { get; set; }
    }
}
