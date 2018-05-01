using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;
using System;
using System.Collections.Generic;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// List of Bp relations ships
    /// </summary>
    public class BpRelationshipsMcfResponse : IMcfResult
    {
        public List<BpRelationship> Results { get; set; }
        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata is ignored
        /// </value>
        public McfMetadata Metadata { get; set; }
    }

    public class BpRelationship
    {
        //public McfMetadata Metadata { get; set; }
        public string AccountID1 { get; set; }
        public string AccountID2 { get; set; }
        public string Message { get; set; }
        public bool Defaultrelationship { get; set; }
        public string Differentiationtypevalue { get; set; }
        public string Relationshipcategory { get; set; }
        public string Relationshiptype { get; set; }
        public string Relationshiptypenew { get; set; }
        public DateTime Validfromdate { get; set; }
        public DateTime? Validfromdatenew { get; set; }
        public DateTime Validtodate { get; set; }
        public DateTime? Validtodatenew { get; set; }
        public string RelDescription { get; set; }
    }
}
