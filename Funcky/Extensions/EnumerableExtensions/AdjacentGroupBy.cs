using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<IEnumerable<TSource>> AdjacentGroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            IEqualityComparer<TKey> comparer = EqualityComparer<TKey>.Default;
            using var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                yield break;
            }

            var group = ImmutableList.Create(enumerator.Current);
            var currentKey = keySelector(enumerator.Current);

            while (enumerator.MoveNext())
            {
                if (comparer.Equals(currentKey, keySelector(enumerator.Current)))
                {
                    group = group.Add(enumerator.Current);
                }
                else
                {
                    yield return group;
                    group = ImmutableList.Create(enumerator.Current);
                    currentKey = keySelector(enumerator.Current);
                }
            }

            yield return group;
        }
    }
}
