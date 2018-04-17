using System.Runtime.Serialization;


namespace PSE.Customer.V1.Repositories.DefinedTypes
{
    /// <summary>
    /// 2.Partner Category (1= Person, 2=Organization)
    /// 3.Partner Role (CRM000=Sold to party (person / org) BUP001= Contact Person
    /// </summary>
    public enum PartnerCategoryType
    {
        /// <summary>
        /// Residential with Partner Role (CRM000=Sold to party (person)
        /// </summary>
        [EnumMember(Value = "Residential")]
        Residential,

        /// <summary>
        /// Organization with Partner Role (CRM000=Sold  org)
        /// </summary>
        [EnumMember(Value = "Organization")]
        Organization,

        /// <summary>
        ///Residential with Partner Role  BUP001= Contact Person
        /// </summary>
        [EnumMember(Value = "AuthorizedContact")]
        AuthorizedContact
    }
}
