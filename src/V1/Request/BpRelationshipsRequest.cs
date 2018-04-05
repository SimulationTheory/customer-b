using PSE.WebAPI.Core.Interfaces;
using System;

namespace PSE.Customer.V1.Request
{
    public class BpRelationshipsRequest : IAPIRequest
    {
    }

    public class BpRelationshipRequest : IAPIRequest
    {
        public string ParenttBpId { get; set; }
        public string BpId { get; set; }
        public string Message { get; set; }
        public string Defaultrelationship { get; set; }
        public string Differentiationtypevalue { get; set; }
        public string Relationshipcategory { get; set; }
        public string Relationshiptype { get; set; }
        public string Relationshiptypenew { get; set; }
        public string Validfromdate { get; set; }
        public DateTime Validfromdatenew { get; set; }
        public string Validtodate { get; set; }
        public DateTime Validtodatenew { get; set; }
    }
}