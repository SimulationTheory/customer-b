using System.Collections.Generic;
using PSE.Customer.V1.Models;

namespace PSE.Customer.V1.Response
{
    public class GetOwnerAccountsResponse
    {
        public GetOwnerAccountsResponse()
        {
            OwnerAccounts = new List<OwnerAccountModel>();
        }

        public IEnumerable<OwnerAccountModel> OwnerAccounts { get; set; }
    }
}
