using System;
using System.Collections.Generic;

namespace PSE.Customer.Extensions
{
    /// <summary>
    ///  Extends IEnumerable Interface
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// It will loop through an Enummerable and excutes an action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
                action(item);
        }
    }
}
