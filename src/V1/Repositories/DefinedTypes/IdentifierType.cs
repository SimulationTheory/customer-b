using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Repositories.DefinedTypes
{
    /// <summary>
    /// Represents Identifier ypes
    /// </summary>
    public enum IdentifierType
    {
        /// <summary>
        /// Work phone
        /// </summary>
        [EnumMember(Value = "ZDOB")]
        ZDOB,

        /// <summary>
        /// Home phone
        /// </summary>
        [EnumMember(Value = "ZLAST4")]
        ZLAST4,

        /// <summary>
        /// Cell phone
        /// </summary>
        [EnumMember(Value = "ZDNAC")]
        ZDNAC
    }
}
