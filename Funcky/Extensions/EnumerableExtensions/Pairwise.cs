using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<(TSource Front, TSource Back)> Pairwise<TSource>(this IEnumerable<TSource> source)
            => Pairwise(source, ValueTuple.Create);

        [Pure]
        public static IEnumerable<TResult> Pairwise<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> resultSelector)
        {
            using var enumerator = source.GetEnumerator();

            if (enumerator.MoveNext())
            {
                for (var previous = enumerator.Current; enumerator.MoveNext(); previous = enumerator.Current)
                {
                    yield return resultSelector(previous, enumerator.Current);
                }
            }
        }
    }
}
