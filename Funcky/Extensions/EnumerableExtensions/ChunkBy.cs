using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<IEnumerable<TSource>> ChunkBy<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => ChunkBy(source, predicate, x => x);

        [Pure]
        public static IEnumerable<TResult> ChunkBy<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<IEnumerable<TSource>, TResult> resultSelector)
        {
            while (source.Any())
            {
                yield return resultSelector(source.TakeWhile(predicate));
                source = source.SkipWhile(Functional.Not(predicate));
            }
        }
    }
}
