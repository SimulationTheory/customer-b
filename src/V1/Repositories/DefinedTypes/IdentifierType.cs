using System.Runtime.Serialization;

namespace PSE.Customer.V1.Repositories.DefinedTypes
{
    /// <summary>
    /// Represents identifier types such as last 4 SSN, drivers license number, etc.
    /// </summary>
    public enum IdentifierType
    {
        // ReSharper disable InconsistentNaming
        
        /// <summary>
        /// Date of birth
        /// </summary>
        [EnumMember(Value = "ZDOB")]
        ZDOB,

        /// <summary>
        /// Driver's License Number and State of Issue
        /// </summary>
        [EnumMember(Value = "ZDRLNO")]
        ZDRLNO,

        /// <summary>
        /// Passport Number ???
        /// </summary>
        [EnumMember(Value = "ZDNAC")]
        ZDNAC,

        /// <summary>
        /// Fraud
        /// </summary>
        [EnumMember(Value = "ZFRAUD")]
        ZFRAUD,

        /// <summary>
        /// Last four digits of Social Security Number
        /// </summary>
        [EnumMember(Value = "ZLAST4")]
        ZLAST4,
        
        /// <summary>
        /// Email Address.
        /// </summary>
        [EnumMember(Value = "ZMAIL")]
        ZMAIL,

        /// <summary>
        /// Military ID
        /// </summary>
        [EnumMember(Value = "ZMILID")]
        ZMILID,

        /// <summary>
        /// Privacy Password
        /// </summary>
        [EnumMember(Value = "ZPASWD")]
        ZPASWD,

        /// <summary>
        /// Federal Tax ID (EIN)
        /// </summary>
        [EnumMember(Value = "ZTAXID")]
        ZTAXID,

        /// <summary>
        /// Washington State Unified Business Identifier (UBI)
        /// </summary>
        [EnumMember(Value = "ZUBI")]
        ZUBI
    }
}
