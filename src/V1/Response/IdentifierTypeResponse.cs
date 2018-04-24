using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PSE.Customer.V1.Repositories.DefinedTypes;

namespace PSE.Customer.V1.Response
{
    /// <summary>
    /// Wrapper for IdentifierType enumerated type
    /// </summary>
    public class IdentifierTypeResponse
    {
        /// <summary>
        /// Gets or sets the type of the identifier.
        /// </summary>
        /// <value>
        /// The type of the identifier:
        /// ZDOB - Date of birth
        /// ZDRLNO - Driver's License Number and State of Issue
        /// ZDNAC - Passport Number ???
        /// ZFRAUD - Fraud
        /// ZLAST4 - Last four digits of Social Security Number
        /// ZMILID - Military ID
        /// ZPASWD - Privacy Password
        /// ZTAXID - Federal Tax ID (EIN)
        /// ZUBI - Washington State Unified Business Identifier (UBI)
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public IdentifierType IdentifierType { get; set; }
    }
}
