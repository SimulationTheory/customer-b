using System.Collections.Generic;
using PSE.Customer.V1.Models;

namespace PSE.Customer.V1.Response
{
    public class GetPremisesResponse
    {
        public GetPremisesResponse()
        {
            Premises = new List<PremiseModel>();
        }

        public IEnumerable<PremiseModel> Premises { get; set; }
    }
}
