

using PSE.Customer.V1.Models;
using PSE.WebAPI.Core.Interfaces;
using System.Collections.Generic;

namespace PSE.Customer.V1.Response
{
    public class IndentifierResponse :IAPIResponse
    {
        public List<IdentifierModel> Identifiers { get; set; }
    }
}
