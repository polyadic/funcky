using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns a sequence mapping each element into a type which has an IsLast property which is true for the last element of the sequence. The returned struct is deconstructible.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <returns>Returns a sequence mapping each element into a type which has an IsLast property which is true for the last element of the sequence.</returns>
        [Pure]
        public static IEnumerable<ValueWithLast<TSource>> WithLast<TSource>(this IEnumerable<TSource> source)
        {
            using var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                yield break;
            }

            var current = enumerator.Current;
            while (enumerator.MoveNext())
            {
                yield return new ValueWithLast<TSource>(current, false);
                current = enumerator.Current;
            }

            yield return new ValueWithLast<TSource>(current, true);
        }

        public readonly struct ValueWithLast<TValue>
        {
            public ValueWithLast(TValue value, bool isLast)
            {
                Value = value;
                IsLast = isLast;
            }

            public TValue Value { get; }

            public bool IsLast { get; }

            public void Deconstruct(out TValue value, out bool isLast)
            {
                value = Value;
                isLast = IsLast;
            }
        }
    }
}
