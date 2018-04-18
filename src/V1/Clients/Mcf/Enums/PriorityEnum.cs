using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System.Runtime.Serialization;


namespace PSE.Customer.V1.Clients.Mcf.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PriorityEnum
    { 
        /// <summary>
        /// Email
        /// </summary>
        [EnumMember(Value = "1")]
        VeryHigh = 1,

        /// <summary>
        /// Mobile phone
        /// </summary>
        [EnumMember(Value = "2")]
        High = 2,

        /// <summary>
        /// Phone
        /// </summary>
        [EnumMember(Value = "3")]
        Average = 3,

        /// <summary>
        /// Fax
        /// </summary>
        [EnumMember(Value = "4")]
        Low = 4,

        /// <summary>
        /// Fax
        /// </summary>
        [EnumMember(Value = "5")]
        VeryLow = 4,
    }
}
