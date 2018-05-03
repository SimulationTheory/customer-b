using System;
using System.Collections.Generic;

namespace PSE.Customer.V1.Request
{
    /// <summary>
    /// Represents a stop service request.
    /// </summary>
    public class MoveOutStopServiceRequest
    {
        /// <summary>
        /// The contract account identifier.
        /// </summary>
        /// <value>
        /// The contract account identifier.
        /// </value>
        public long ContractAccountId { get; set; }

        /// <summary>
        /// The move out date.
        /// </summary>
        /// <value>
        /// The move out date.
        /// </value>
        public DateTimeOffset MoveOutDate { get; set; }

        /// <summary>
        /// The installation ids.
        /// </summary>
        /// <value>
        /// The installation ids.
        /// </value>
        public List<long> InstallationIds { get; set; }
    }
}
