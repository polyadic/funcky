using System;
using System.Collections.Generic;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// The IEnumerable version of foreach. You can apply an action to each element. This is only useful when you have side effects.
        /// </summary>
        /// <typeparam name="T">the inner type of the enumerable.</typeparam>
        public static void ForEach<T>(this IEnumerable<T> elements, Action<T> action)
        {
            foreach (var element in elements)
            {
                action(element);
            }
        }
    }
}
