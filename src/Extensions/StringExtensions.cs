using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PSE.Customer.Extensions
{
    /// <summary>
    /// Extends string class
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Extension method to convert a string into an enum of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns>T</returns>
        public static T ToEnum<T>(this string str)
        {
            var enumType = typeof(T);
            foreach (var name in Enum.GetNames(enumType))
            {
                var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
                if (enumMemberAttribute.Value == str) return (T)Enum.Parse(enumType, name);
            }
            return default(T);
        }

        /// <summary>
        /// Helpler method to convert object to JSON (e.g. for logging objects)
        /// </summary>
        /// <param name="serializableObject">The serializable object.</param>
        /// <param name="formatting">The formatting style (defaults to indented)</param>
        /// <returns>The object formatted as a JSON string</returns>
        public static string ToJson(this object serializableObject, Formatting formatting = Formatting.Indented)
        {
            return JsonConvert.SerializeObject(serializableObject, formatting);
        }
    }
}
