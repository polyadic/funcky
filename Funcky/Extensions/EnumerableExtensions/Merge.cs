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
        /// <summary>
        /// Merges two ordered sequences into one and preserves the ordering. The merged sequences has exactly the same number of elements as the inputs combined.
        /// </summary>
        /// <remarks>PRECONDITION: The given sequences must be ordered by the same ordering as the given IComparer.</remarks>
        /// <param name="sequence1">First ordered sequence.</param>
        /// <param name="sequence2">Second ordered sequence.</param>
        /// <param name="comparer">The comparer giving the order condition of the sequences.</param>
        /// <typeparam name="TSource">The type elements in the sequences.</typeparam>
        /// <returns>The merged sequences in the same order as the given sequences.</returns>
        [Pure]
        public static IEnumerable<TSource> Merge<TSource>(this IEnumerable<TSource> sequence1, IEnumerable<TSource> sequence2, Option<IComparer<TSource>> comparer = default)
            => ImmutableList.Create(sequence1, sequence2).Merge(comparer);

        /// <summary>
        /// Merges three ordered sequences into one and preserves the ordering. The merged sequences has exactly the same number of elements as the inputs combined.
        /// </summary>
        /// <remarks>PRECONDITION: The given sequences must be ordered by the same ordering as the given IComparer.</remarks>
        /// <param name="sequence1">First ordered sequence.</param>
        /// <param name="sequence2">Second ordered sequence.</param>
        /// <param name="sequence3">Third ordered sequence.</param>
        /// <param name="comparer">The comparer giving the order condition of the sequences.</param>
        /// <typeparam name="TSource">The type elements in the sequences.</typeparam>
        /// <returns>The merged sequences in the same order as the given sequences.</returns>
        [Pure]
        public static IEnumerable<TSource> Merge<TSource>(this IEnumerable<TSource> sequence1, IEnumerable<TSource> sequence2, IEnumerable<TSource> sequence3, Option<IComparer<TSource>> comparer = default)
            => ImmutableList.Create(sequence1, sequence2, sequence3).Merge(comparer);

        /// <summary>
        /// Merges three ordered sequences into one and preserves the ordering. The merged sequences has exactly the same number of elements as the inputs combined.
        /// </summary>
        /// <remarks>PRECONDITION: The given sequences must be ordered by the same ordering as the given IComparer.</remarks>
        /// <param name="sequence1">First ordered sequence.</param>
        /// <param name="sequence2">Second ordered sequence.</param>
        /// <param name="sequence3">Third ordered sequence.</param>
        /// <param name="sequence4">Forth ordered sequence.</param>
        /// <param name="comparer">The comparer giving the order condition of the sequences.</param>
        /// <typeparam name="TSource">The type elements in the sequences.</typeparam>
        /// <returns>The merged sequences in the same order as the given sequences.</returns>
        [Pure]
        public static IEnumerable<TSource> Merge<TSource>(this IEnumerable<TSource> sequence1, IEnumerable<TSource> sequence2, IEnumerable<TSource> sequence3, IEnumerable<TSource> sequence4, Option<IComparer<TSource>> comparer = default)
            => ImmutableList.Create(sequence1, sequence2, sequence3, sequence4).Merge(comparer);

        /// <summary>
        /// Merges a sequence of ordered sequences into one and preserves the ordering. The merged sequences has exactly the same number of elements as the inputs combined.
        /// </summary>
        /// <remarks>PRECONDITION: The given sequences must be ordered by the same ordering as the given IComparer.</remarks>
        /// <param name="sources">First ordered sequence.</param>
        /// <param name="comparer">The comparer giving the order condition of the sequences.</param>
        /// <typeparam name="TSource">The type elements in the sequences.</typeparam>
        /// <returns>The merged sequences in the same order as the given sequences.</returns>
        [Pure]
        public static IEnumerable<TSource> Merge<TSource>(this IEnumerable<IEnumerable<TSource>> sources, Option<IComparer<TSource>> comparer = default)
        {
            var enumerators = GetMergeEnumerators(sources);

            try
            {
                return MergeEnumerators(enumerators.RemoveAll(MoveNextMerge), GetMergeComparer(comparer));
            }
            finally
            {
                enumerators.ForEach(s => s.Dispose());
            }
        }

        private static ImmutableList<IEnumerator<TSource>> GetMergeEnumerators<TSource>(IEnumerable<IEnumerable<TSource>> sources)
            => ImmutableList.Create<IEnumerator<TSource>>().AddRange(sources.Select(s => s.GetEnumerator()));

        private static IComparer<TSource> GetMergeComparer<TSource>(Option<IComparer<TSource>> comparer = default)
            => comparer.GetOrElse(Comparer<TSource>.Default);

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
