using PSE.WebAPI.Core.Interfaces;
using System;

namespace PSE.Customer.V1.Request
{
    public class BpRelationshipsRequest :  IAPIRequest
    {

        public string AccountID1 { get; set; }
        public string AccountID2 { get; set; }
        public bool Defaultrelationship { get; set; } 
        public string Differentiationtypevalue { get; set; } = "";
        public string Relationshipcategory { get; set; }
        public DateTime? Validfromdatenew { get; set; }
       
        public DateTime? Validtodatenew { get; set; }
    }
}