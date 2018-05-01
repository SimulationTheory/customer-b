

using System.Runtime.Serialization;

namespace PSE.Customer.V1.Repositories.DefinedTypes
{
    public enum RelationshipCategory
    {

        // <summary>
        /// Is Married To
        ///</summary>
        [EnumMember(Value = "BUR004")]
        Spouse,

        // <summary>
        /// Has Shared Living Arrangement Member
        ///</summary>
        [EnumMember(Value = "BUR003")]
        Roommate,

        // <summary>
        /// Property Manager
        ///</summary>
        [EnumMember(Value = "ZPMGR")]
        PropertyManager,

        /// <summary>
        /// Has the Employee
        /// </summary>
        [EnumMember(Value = "BUR010")]
        Employee,

        /// <summary>
        ///Has the President
        /// </summary>
        [EnumMember(Value = "ZPRES")]
        President,

        /// <summary>
        /// Authorized Third Party
        /// </summary>
        [EnumMember(Value = "CHM004")]
        Other
    }
}
