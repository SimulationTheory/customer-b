using PSE.WebAPI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Response
{
    public class BpRelationshipsResponse : IAPIResponse
    {
        public string BpId { get; set; }
        public List<BpRelationshipResponse> Relationships { get; set; }
    }

    public class BpRelationshipResponse
    {
        public string BpIdParent { get; set; }
        public string BpId { get; set; }
        public string Relationshipcategory { get; set; }
        public string Message { get; set; }
    }
}
