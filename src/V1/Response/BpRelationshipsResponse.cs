using PSE.WebAPI.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace PSE.Customer.V1.Response
{
    public class BpRelationshipsResponse : IAPIResponse
    {
        public List<BpRelationshipResponse> RelationShips{get; set;}
    }

    public  class BpRelationshipResponse
    {
        public string BpId1{ get; set; }
        public string BpId2 { get; set; }
        public string Relationshipcategory { get; set; }
        public string Message { get; set; }
        public DateTime Validfromdate { get; set; }      
        public DateTime Validtodate { get; set; }
    }
}