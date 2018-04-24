using System.Collections.Generic;
using PSE.Customer.V1.Models;
using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Response
{
    /// <summary>
    /// Response that contains information about installations for a given premise.
    /// </summary>
    /// <seealso cref="PSE.WebAPI.Core.Interfaces.IAPIResponse" />
    public class GetPremiseInstallationsResponse : IAPIResponse
    {
        /// <summary>
        /// Gets or sets the premise identifier.
        /// </summary>
        /// <value>
        /// The premise identifier.
        /// </value>
        public long PremiseId { get; set; }
        /// <summary>
        /// Gets or sets the type of the premise.
        /// </summary>
        /// <value>
        /// The type of the premise.
        /// </value>
        public string PremiseType { get; set; }
        /// <summary>
        /// Gets or sets the storm mode act.
        /// </summary>
        /// <value>
        /// The storm mode act.
        /// </value>
        public string StormModeAct { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether a premise is  ineligible for movein.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [ineligible for movein]; otherwise, <c>false</c>.
        /// </value>
        public bool IneligibleForMovein { get; set; }
        /// <summary>
        /// A list of installations.
        /// </summary>
        /// <value>
        /// The installations.
        /// </value>
        public List<Installation> Installations { get; set; }
    }
}
