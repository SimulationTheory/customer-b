using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PSE.Customer.V1.Clients.Mcf.Enums
{
    /// <summary>
    /// Passed to SAP to identify phone type not tied to a location
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AddressIndependantContactInfoEnum
    {
        /// <summary>
        /// Email
        /// </summary>
        [EnumMember(Value = "1")]
        AccountAddressIndependentEmails = 1,

        /// <summary>
        /// Mobile phone
        /// </summary>
        [EnumMember(Value = "2")]
        AccountAddressIndependentMobilePhones = 2,

        /// <summary>
        /// Phone
        /// </summary>
        [EnumMember(Value = "3")]
        AccountAddressIndependentPhones = 3,

        /// <summary>
        /// Fax
        /// </summary>
        [EnumMember(Value = "4")]
        AccountAddressIndependentFaxes = 4
    }
}