using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace PSE.Customer.Extensions
{
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

    }
}
