using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// An IEnumerable that calls a function on each element before yielding it. It can be used to encode side effects without enumerating.
        /// The side effect will be executed when enumerating the result.
        /// </summary>
        /// <typeparam name="T">the inner type of the enumerable.</typeparam>
        /// <returns>returns an <see cref="IEnumerable{T}" /> with the side effect defined by action encoded in the enumerable.</returns>
        [Pure]
        public static async IAsyncEnumerable<T> Inspect<T>(this IAsyncEnumerable<T> elements, Action<T> action)
        {
            await foreach (var element in elements)
            {
                action(element);
                yield return element;
            }
        }
    }
}
