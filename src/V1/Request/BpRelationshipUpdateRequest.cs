using Newtonsoft.Json;
using PSE.RestUtility.Core.Json;
using PSE.WebAPI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Request
{
    public class BpRelationshipUpdateRequest : IAPIRequest
    {
        /// <summary>
        /// Gets or sets the account id
        /// </summary>
        /// <value>
        ///   The business partner identifier.
        /// </value>
        [JsonConverter(typeof(ToStringJsonConverter))]
        public long AccountID1 { get; set; }

        /// <summary>
        /// Gets or sets the new relationship BP
        /// </summary>
        /// <value>
        ///   The updated business partner id for relationship
        /// </value>
        [JsonConverter(typeof(ToStringJsonConverter))]
        public long AccountID2 { get; set; }

        /// <summary>
        /// Gets or sets the tenant Bp Id to be used to update relationship
        /// we will use it in place of AccountID1 aboe in Landlord scenarios
        /// </summary>
        /// <value>
        ///   The business partner identifier.
        /// </value>
        [JsonConverter(typeof(ToStringJsonConverter))]
        public long TenantBpId { get; set; }

        /// <summary>
        /// Gets or sets the note about BP update
        /// </summary>
        /// <value>
        ///   The note about update of business partner relationship
        /// </value>
        public string Message { get; set; } = String.Empty;

        /// <summary>
        /// Specifies if this is the default relationship with BP
        /// </summary>
        /// <value>
        ///   true or falso value depending on whether this is default BP relationship
        /// </value>
        public bool Defaultrelationship { get; set; } 

        /// <summary>
        /// Gets or sets the differentiation type value
        /// </summary>
        /// <value>
        ///   The differentiation type value
        /// </value>
        [JsonConverter(typeof(ToStringJsonConverter))]
        public string Differentiationtypevalue { get; set; }

        /// <summary>
        /// Gets or sets the relationship category
        /// </summary>
        /// <value>
        ///   the relationship category value
        /// </value>
        public string Relationshipcategory { get; set; }

        /// <summary>
        /// Gets or sets the relationship type
        /// </summary>
        /// <value>
        ///   holds the relationship category
        /// </value>
        //public string Relationshiptype { get; set; } = String.Empty;

        /// <summary>
        /// Gets or sets the new relationship type
        /// </summary>
        /// <value>
        ///   holds the new relationship category
        /// </value>
        public string Relationshiptypenew { get; set; }

        /// <summary>
        /// Gets or sets the valid date from
        /// </summary>
        /// <value>
        ///   the valid date from for new BP relationship
        /// </value>
        public string Validfromdate { get; set; } = "";

        /// <summary>
        /// Gets or sets the valid date from
        /// </summary>
        /// <value>
        ///   the valid date from for new BP relationship
        /// </value>
        public string Validfromdatenew { get; set; } = "";

        /// <summary>
        /// Gets or sets the valid date to
        /// </summary>
        /// <value>
        ///   the valid date to for BP relationship
        /// </value>
        public string Validtodate { get; set; } = "";

        /// <summary>
        /// Gets or sets the new valid date to
        /// </summary>
        /// <value>
        ///   the valid date to for new BP relationship
        /// </value>
        public string Validtodatenew { get; set; } = "";
    }
}
