using System.Runtime.Serialization;

namespace PSE.Customer.V1.Repositories.DefinedTypes
{
    /// <summary>
    /// Defines the type of the phone being passed in
    /// </summary>
    public enum PhoneType
    {
        /// <summary>
        /// Work phone
        /// </summary>
        [EnumMember(Value = "work")]
        Work,

        /// <summary>
        /// Home phone
        /// </summary>
        [EnumMember(Value = "home")]
        Home,

        /// <summary>
        /// Cell phone
        /// </summary>
        [EnumMember(Value = "cell")]
        Cell
    }
}
