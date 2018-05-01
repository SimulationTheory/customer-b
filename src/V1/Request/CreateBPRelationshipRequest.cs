using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Request
{
    public class CreateBPRelationshipRequest
    {
        public string AccountID1 { get; set; }

        public string AccountID2 { get; set; }
        public string Message { get; set; }
        public bool Defaultrelationship { get; set; }
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
