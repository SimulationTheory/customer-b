using System.Runtime.Serialization;

namespace PSE.Customer.V1.Repositories.DefinedTypes
{
    /// <summary>
    /// Represents BP identifier types such as last 4 SSN, drivers license number, etc.
    /// </summary>
    public enum IdentifierType
    {
        // ReSharper disable InconsistentNaming
        
        /// <summary>
        /// Creditor Identifier
        /// </summary>
        [EnumMember(Value = "BKK100")]
        BKK100,

        /// <summary>
        /// Dun and Bradstreet Number
        /// </summary>
        [EnumMember(Value = "BUP001")]
        BUP001,

        /// <summary>
        /// Commercial Register Number
        /// </summary>
        [EnumMember(Value = "BUP002")]
        BUP002,

        /// <summary>
        /// Register of Associations Number
        /// </summary>
        [EnumMember(Value = "BUP003")]
        BUP003,

        /// <summary>
        /// Public Register of Cooperatives Number
        /// </summary>
        [EnumMember(Value = "BUP004")]
        BUP004,

        /// <summary>
        /// Global Location Number
        /// </summary>
        [EnumMember(Value = "BUP005")]
        BUP005,

        /// <summary>
        /// Standard Carrier Alpha Code
        /// </summary>
        [EnumMember(Value = "BUP006")]
        BUP006,

        /// <summary>
        /// Counterparty number
        /// </summary>
        [EnumMember(Value = "CPID01")]
        CPID01,

        /// <summary>
        ///  R/3 Customer Number
        /// </summary>
        [EnumMember(Value = "CRM002")]
        CRM002,

        /// <summary>
        /// ID Card
        /// </summary>
        [EnumMember(Value = "FS0001")]
        FS0001,

        /// <summary>
        /// Passport
        /// </summary>
        [EnumMember(Value = "FS0002")]
        FS0002,

        /// <summary>
        /// Social Security Number
        /// </summary>
        [EnumMember(Value = "IBS001")]
        IBS001,

        /// <summary>
        /// Desc/Business for 3rd Party
        /// </summary>
        [EnumMember(Value = "ZADD1")]
        ZADD1,

        /// <summary>
        /// Desc/Business for 3rd Party Cont.
        /// </summary>
        [EnumMember(Value = "ZADD2")]
        ZADD2,

        /// <summary>
        /// Special Customer
        /// </summary>
        [EnumMember(Value = "ZCSNR")]
        ZCSNR,

        /// <summary>
        /// Co-Customer Business Agreement ID
        /// </summary>
        [EnumMember(Value = "ZDCOCU")]
        ZDCOCU,

        /// <summary>
        /// Not accepting checks
        /// </summary>
        [EnumMember(Value = "ZDNAC")]
        ZDNAC,

        /// <summary>
        /// EDI Number
        /// </summary>
        [EnumMember(Value = "ZEDI")]
        ZEDI,

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
        /// Employer
        /// </summary>
        [EnumMember(Value = "ZEMPLO")]
        ZEMPLO,

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
        /// Return Mail
        /// </summary>
        [EnumMember(Value = "ZMAIL")]
        ZMAIL,

        /// <summary>
        /// Major Accounts
        /// </summary>
        [EnumMember(Value = "ZMAJA")]
        ZMAJA,

        /// <summary>
        /// Managed Account
        /// </summary>
        [EnumMember(Value = "ZMANA")]
        ZMANA,

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
        /// RCM Customers
        /// </summary>
        [EnumMember(Value = "ZRCM")]
        ZRCM,

        /// <summary>
        /// Years of Service
        /// </summary>
        [EnumMember(Value = "ZSYEAR")]
        ZSYEAR,

        /// <summary>
        /// Federal Tax ID (EIN)
        /// </summary>
        [EnumMember(Value = "ZTAXID")]
        ZTAXID,

        /// <summary>
        /// Washington State Unified Business Identifier (UBI)
        /// </summary>
        [EnumMember(Value = "ZUBI")]
        ZUBI,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember(Value = "WUTC Complaint")]
        ZWUTC
    }
}
