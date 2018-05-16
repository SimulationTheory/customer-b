using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PSE.WebAPI.Core.Service.Enums;
using PSE.WebAPI.Core.Service.Interfaces;

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
        /// Helper method to convert object to JSON
        /// </summary>
        /// <param name="serializableObject">The serializable object.</param>
        /// <param name="formatting">The formatting style (defaults to indented)</param>
        /// <param name="nullValueHandling">How to serialze null values (defaults to ignore)</param>
        /// <returns>The object formatted as a JSON string</returns>
        public static string ToJson(this object serializableObject, Formatting formatting = Formatting.Indented, NullValueHandling nullValueHandling = NullValueHandling.Ignore)
        {
            var json = JsonConvert.SerializeObject(serializableObject, formatting,
                new JsonSerializerSettings { NullValueHandling = nullValueHandling });
            return json;
        }

        /// <summary>
        /// TODO: Add JsonIgnore attribute to interface and remove this class
        /// </summary>
        public class RequestContextAdapterFormatHelper
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RequestContextAdapterFormatHelper"/> class.
            /// </summary>
            /// <param name="requstContext">The requst context.</param>
            public RequestContextAdapterFormatHelper(IRequestContextAdapter requstContext)
            {
                RequestChannel = requstContext.RequestChannel;
                if (!string.IsNullOrWhiteSpace(requstContext.JWT))
                {
                    UserId = requstContext.UserId;
                }
            }

            /// <summary>
            /// Gets the request channel.
            /// </summary>
            /// <value>
            /// The request channel.
            /// </value>
            public RequestChannelEnum RequestChannel { get; }

            /// <summary>
            /// Gets the user identifier.
            /// </summary>
            /// <value>
            /// The user identifier.
            /// </value>
            public Guid UserId { get; }
        }

        /// <summary>
        /// Helper method to convert object to JSON (e.g. for logging objects)
        /// </summary>
        /// <param name="requstContext">The serializable object.</param>
        /// <param name="formatting">The formatting style (defaults to indented)</param>
        /// <returns>The object formatted as a JSON string</returns>
        public static string ToJson(this IRequestContextAdapter requstContext, Formatting formatting = Formatting.Indented)
        {
            return ToJson(new RequestContextAdapterFormatHelper(requstContext), formatting);
        }
    }
}
