using System;
using System.Collections.Generic;

namespace Funcky.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Wraps this object instance into an IEnumerable&lt;T&gt;
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="item"> The instance that will be wrapped. </param>
        /// <returns> An <see cref="IEnumerable{T}" /> consisting of a single item. </returns>
        public static IEnumerable<T> Yield<T>(this T item)
        {
            if (item is { })
            {
                yield return item;
            }
        }

        /// <summary>
        /// An IEnumerable that calls a function on each element before yielding it. It can be used to encode side effects without enumerating.
        /// The side effect will be executed when enumerating the result.
        /// </summary>
        /// <typeparam name="T">the inner type of the enumerable.</typeparam>
        /// <returns>returns an <see cref="IEnumerable{T}" /> with the sideeffect defined by action encoded in the enumerable.</returns>
        public static IEnumerable<T> Inspect<T>(this IEnumerable<T> elements, Action<T> action)
        {
            foreach (var element in elements)
            {
                action(element);
                yield return element;
            }
        }

        /// <summary>
        /// The IEnumerable version of foreach. You can apply an action to each element. This is only useful when you have side effects.
        /// </summary>
        /// <typeparam name="T">the inner type of the enumerable.</typeparam>
        public static void Each<T>(this IEnumerable<T> elements, Action<T> action)
        {
            foreach (var element in elements)
            {
                action(element);
            }
        }
    }
}
