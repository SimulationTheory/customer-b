using PSE.WebAPI.Core.Interfaces;
using System.Collections.Generic;

namespace PSE.Customer.V1.Response
{
    public class BpRelationshipsResponse : IAPIResponse
    {
        public string BpId { get; set; }
        public List<BpRelationshipResponse> RelationShips{get; set;}
    }

    public  class BpRelationshipResponse
    {
        public string BpIdParent{ get; set; }
        public string BpId { get; set; }
        public string Relationshipcategory { get; set; }
        public string Message { get; set; }
    }
}