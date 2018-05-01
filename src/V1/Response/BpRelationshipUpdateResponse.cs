using PSE.WebAPI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Response
{
    
    public class BpRelationshipUpdateResponse : IAPIResponse
    {
        /// <summary>
        /// Gets or sets the status code
        /// </summary>
        /// <value>
        ///   The return value of the status code.
        /// </value>
        public string status_code { get; set; }
        /// <summary>
        /// Gets or sets the status reason
        /// </summary>
        /// <value>
        ///   The return value of the status reason.
        /// </value>
        public string status_reason { get; set; }
    }
}
