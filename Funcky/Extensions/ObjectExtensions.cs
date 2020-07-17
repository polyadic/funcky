using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funcky.GenericConstraints;

namespace Funcky.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Wraps this object instance into an <see cref="IEnumerable{T}"/>
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}" /> consisting of a single item or zero items.</returns>
        [Pure]
        public static IEnumerable<T> ToEnumerable<T>(this T? item, RequireClass<T>? ω = null)
            where T : class
        {
            if (item is { })
            {
                yield return item;
            }
        }

        /// <inheritdoc cref="ToEnumerable{T}(T, RequireClass{T})"/>
        [Pure]
        public static IEnumerable<T> ToEnumerable<T>(this T item, RequireStruct<T>? ω = null)
            where T : struct
        {
            yield return item;
        }

        /// <inheritdoc cref="ToEnumerable{T}(T, RequireClass{T})"/>
        [Pure]
        public static IEnumerable<T> ToEnumerable<T>(this T? item)
            where T : struct
        {
            if (item.HasValue)
            {
                yield return item.Value;
            }
        }

        public static TResult Then<TInput, TResult>(this TInput value, Func<TInput, TResult> func) => func(value);
    }
}
