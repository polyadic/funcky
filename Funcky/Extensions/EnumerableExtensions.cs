#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Funcky.Constraints;
using Funcky.GenericConstraints;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Wraps this object instance into an <see cref="IEnumerable{T}"/>
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <returns>An <see cref="IEnumerable{T}" /> consisting of a single item or zero items.</returns>
        public static IEnumerable<T> Yield<T>(this T? item, RequireClass<T>? ω = null)
            where T : class
        {
            if (item is { })
            {
                yield return item;
            }
        }

        /// <inheritdoc cref="Yield{T}(T, RequireClass{T})"/>
        public static IEnumerable<T> Yield<T>(this T item, RequireStruct<T>? ω = null)
            where T : struct
        {
            yield return item;
        }

        /// <inheritdoc cref="Yield{T}(T, RequireClass{T})"/>
        public static IEnumerable<T> Yield<T>(this T? item)
            where T : struct
        {
            if (item.HasValue)
            {
                yield return item.Value;
            }
        }

        /// <summary>
        /// Projects and filters an <see cref="IEnumerable{T}"/> at the same time.
        /// This is done by filtering out any empty <see cref="Option{T}"/> values returned by the <paramref name="selector"/>.
        /// </summary>
        public static IEnumerable<TOutput> WhereSelect<TInput, TOutput>(this IEnumerable<TInput> inputs, Func<TInput, Option<TOutput>> selector)
            where TOutput : notnull
            => inputs.SelectMany(input => selector(input).ToEnumerable());

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
