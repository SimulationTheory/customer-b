using PSE.WebAPI.Core.Interfaces;
using System;

namespace PSE.Customer.V1.Request
{
    using Newtonsoft.Json;

    public class BpRelationshipsRequest : IAPIRequest
    {
    }

    public class BpRelationshipRequest : IAPIRequest
    {
        public string AccountID1 { get; set; }

        public string AccountID2 { get; set; }
        public string Message { get; set; }
        public bool Defaultrelationship { get; set; }
        public string Differentiationtypevalue { get; set; }
        public string Relationshipcategory { get; set; }
        public string Relationshiptype { get; set; }
        public string Relationshiptypenew { get; set; }
        public DateTimeOffset Validfromdate { get; set; }
        public DateTimeOffset Validfromdatenew { get; set; }
        public DateTimeOffset Validtodate { get; set; }
        public DateTimeOffset Validtodatenew { get; set; }
    }
}