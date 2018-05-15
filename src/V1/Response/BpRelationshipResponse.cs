using PSE.WebAPI.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace PSE.Customer.V1.Response
{

    public class BpRelationResponse : IAPIResponse
    {
        public List<Bprelationship> BpRelationShips { get; set; }
    }
    public class BpRelationshipsResponse
    {    
        public List<BpRelationshipResponse> RelationShips{get; set;}
    }

    public  class BpRelationshipResponse
    {
        public string PrimaryBpId{ get; set; }
        public string AuthorizedBpId { get; set; }
        public string Relationshipcategory { get; set; }
        public string Message { get; set; }
        public string Validfromdate { get; set; }
        public string Validfromdatenew { get; set; }
        public string Validtodate { get; set; }
        public string Validtodatenew { get; set; }
    }

    public class Bprelationship
    {
        public string PrimaryBpId { get; set; }
        public string AuthorizedBpId { get; set; }
        public string Relationshipcategory { get; set; }
    }
}