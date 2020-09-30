using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<TSource> Merge<TSource>(this IEnumerable<TSource> sequence1, IEnumerable<TSource> sequence2, IComparer<TSource>? comparer = null)
            => ImmutableList.Create(sequence1).Add(sequence2).Merge(comparer);

        [Pure]
        public static IEnumerable<TSource> Merge<TSource>(this IEnumerable<TSource> sequence1, IEnumerable<TSource> sequence2, IEnumerable<TSource> sequence3, IComparer<TSource>? comparer = null)
            => ImmutableList.Create(sequence1).Add(sequence2).Add(sequence3).Merge(comparer);

        [Pure]
        public static IEnumerable<TSource> Merge<TSource>(this IEnumerable<TSource> sequence1, IEnumerable<TSource> sequence2, IEnumerable<TSource> sequence3, IEnumerable<TSource> sequence4, IComparer<TSource>? comparer = null)
            => ImmutableList.Create(sequence1).Add(sequence2).Add(sequence3).Add(sequence4).Merge(comparer);

        [Pure]
        public static IEnumerable<TSource> Merge<TSource>(this IEnumerable<IEnumerable<TSource>> sources, IComparer<TSource>? comparer = null)
        {
            var enumerators = GetMergeEnumerators(sources);
            var result = MergeEnumerators(enumerators.RemoveAll(MoveNextMerge), GetMergeComparer(comparer));

            enumerators.ForEach(s => s.Dispose());

            return result;
        }

        private static ImmutableList<IEnumerator<TSource>> GetMergeEnumerators<TSource>(IEnumerable<IEnumerable<TSource>> sources)
            => ImmutableList.Create<IEnumerator<TSource>>().AddRange(sources.Select(s => s.GetEnumerator()));

        private static IComparer<TSource> GetMergeComparer<TSource>(IComparer<TSource>? comparer)
            => Option.FromNullable(comparer).GetOrElse(Comparer<TSource>.Default);

        private static bool MoveNextMerge<TSource>(IEnumerator<TSource> enumerator)
            => !enumerator.MoveNext();

        private static IEnumerable<TSource> MergeEnumerators<TSource>(ImmutableList<IEnumerator<TSource>> enumerators, IComparer<TSource> comparer)
        {
            while (!enumerators.IsEmpty)
            {
                var minimum = FindMinimum(enumerators, comparer);
                yield return minimum.Current;

                enumerators = RemoveYielded(minimum, enumerators);
            }
        }

        private static ImmutableList<IEnumerator<TSource>> RemoveYielded<TSource>(IEnumerator<TSource> minimum, ImmutableList<IEnumerator<TSource>> enumerators) =>
            minimum.MoveNext()
                ? enumerators
                : enumerators.Remove(minimum);

        private static IEnumerator<TSource> FindMinimum<TSource>(ImmutableList<IEnumerator<TSource>> enumerators, IComparer<TSource> comparer)
            => enumerators.Aggregate(Minimum(comparer));

        private static Func<IEnumerator<TSource>, IEnumerator<TSource>, IEnumerator<TSource>> Minimum<TSource>(IComparer<TSource> comparer)
            => (enumerator, minimum) => comparer.Compare(minimum.Current, enumerator.Current) <= 0 ? minimum : enumerator;
    }
}
