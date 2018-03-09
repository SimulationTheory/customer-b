using System.Runtime.Serialization;

namespace PSE.Customer.V1.Repositories.DefinedTypes
{    
    public enum PhoneType
    {
        [EnumMember(Value = "work")]
        Work,
        [EnumMember(Value = "home")]
        Home,
        [EnumMember(Value = "cell")]
        Cell,
    }
}
