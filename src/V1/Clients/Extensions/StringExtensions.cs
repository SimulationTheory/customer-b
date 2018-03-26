using System;

namespace PSE.Customer.V1.Clients.Extensions
{
    /// <summary>
    /// Extensions to transform strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a numeric string to a DateTime
        /// </summary>
        /// <param name="source">Number of seconds or milliseconds since Jan/01/1970</param>
        /// <returns></returns>
        public static DateTime? FromUnixTimeSeconds(this string source)
        {
            if (long.TryParse(source, out var sourceAsLong))
            {
                return DateTimeOffset.FromUnixTimeSeconds(sourceAsLong).DateTime;
            }

            return null;
        }

        /// <summary>
        /// Returns middle of the string between two substrings.
        /// </summary>
        /// <param name="source">The string to parse</param>
        /// <param name="left">The left string</param>
        /// <param name="right">The right string</param>
        /// <returns></returns>
        public static string Between(this string source, string left, string right)
        {
            var result = source;

            var leftPos = source.IndexOf(left, StringComparison.Ordinal);
            if (leftPos >= 0)
            {
                var substring = source.Substring(leftPos + left.Length);
                var rightPos = substring.LastIndexOf(right, StringComparison.Ordinal);
                if (rightPos >= 0)
                {
                    result = substring.Substring(0, rightPos);
                }
            }

            return result;
        }
    }
}
