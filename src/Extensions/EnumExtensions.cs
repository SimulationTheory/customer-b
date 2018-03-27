using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace PSE.Customer.Extensions
{
    /// <summary>
    /// Extends access to enumerated types
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the type of the attribute of.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumVal">The enum value.</param>
        /// <returns></returns>
        public static string GetEnumMemberValue<T>(this T enumVal) where T : struct, IConvertible
        {
            return typeof(T)
                .GetTypeInfo()
                .DeclaredMembers
                .SingleOrDefault(x => x.Name == enumVal.ToString(CultureInfo.InvariantCulture))
                ?.GetCustomAttribute<EnumMemberAttribute>(false)
                ?.Value;
        }
    }
}
