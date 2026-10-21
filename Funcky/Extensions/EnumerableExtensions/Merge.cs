#pragma warning disable RS0026

using System.Collections.Immutable;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <param name="source1">First ordered sequence.</param>
    /// <typeparam name="TSource">The type elements in the sequences.</typeparam>
    extension<TSource>(IEnumerable<TSource> source1)
    {
        /// <summary>
        /// Merges two ordered sequences into one and preserves the ordering. The merged sequences has exactly the same number of elements as the inputs combined.
        /// </summary>
        /// <remarks>PRECONDITION: The given sequences must be ordered by the same ordering as the given IComparer.</remarks>
        /// <param name="source2">Second ordered sequence.</param>
        /// <param name="comparer">The comparer giving the order condition of the sequences.</param>
        /// <returns>The merged sequences in the same order as the given sequences.</returns>
        [Pure]
        public IEnumerable<TSource> Merge(IEnumerable<TSource> source2, Option<IComparer<TSource>> comparer = default)
            => Merge([ArgumentValidator.NotNull(source1), ArgumentValidator.NotNull(source2)], comparer);

        /// <summary>
        /// Merges three ordered sequences into one and preserves the ordering. The merged sequences has exactly the same number of elements as the inputs combined.
        /// </summary>
        /// <remarks>PRECONDITION: The given sequences must be ordered by the same ordering as the given IComparer.</remarks>
        /// <param name="source2">Second ordered sequence.</param>
        /// <param name="source3">Third ordered sequence.</param>
        /// <param name="comparer">The comparer giving the order condition of the sequences.</param>
        /// <returns>The merged sequences in the same order as the given sequences.</returns>
        [Pure]
        public IEnumerable<TSource> Merge(IEnumerable<TSource> source2, IEnumerable<TSource> source3, Option<IComparer<TSource>> comparer = default)
            => Merge([ArgumentValidator.NotNull(source1), ArgumentValidator.NotNull(source2), ArgumentValidator.NotNull(source3)], comparer);

        /// <summary>
        /// Merges three ordered sequences into one and preserves the ordering. The merged sequences has exactly the same number of elements as the inputs combined.
        /// </summary>
        /// <remarks>PRECONDITION: The given sequences must be ordered by the same ordering as the given IComparer.</remarks>
        /// <param name="source2">Second ordered sequence.</param>
        /// <param name="source3">Third ordered sequence.</param>
        /// <param name="source4">Forth ordered sequence.</param>
        /// <param name="comparer">The comparer giving the order condition of the sequences.</param>
        /// <returns>The merged sequences in the same order as the given sequences.</returns>
        [Pure]
        public IEnumerable<TSource> Merge(IEnumerable<TSource> source2, IEnumerable<TSource> source3, IEnumerable<TSource> source4, Option<IComparer<TSource>> comparer = default)
            => Merge([ArgumentValidator.NotNull(source1), ArgumentValidator.NotNull(source2), ArgumentValidator.NotNull(source3), ArgumentValidator.NotNull(source4)], comparer);
    }

    /// <param name="sources">First ordered sequence.</param>
    /// <typeparam name="TSource">The type elements in the sequences.</typeparam>
    extension<TSource>(IEnumerable<IEnumerable<TSource>> sources)
    {
        /// <summary>
        /// Merges a sequence of ordered sequences into one and preserves the ordering. The merged sequences has exactly the same number of elements as the inputs combined.
        /// </summary>
        /// <remarks>PRECONDITION: The given sequences must be ordered by the same ordering as the given IComparer.</remarks>
        /// <param name="comparer">The comparer giving the order condition of the sequences.</param>
        /// <returns>The merged sequences in the same order as the given sequences.</returns>
        [Pure]
        public IEnumerable<TSource> Merge(Option<IComparer<TSource>> comparer = default)
            => MergeIterator(ArgumentValidator.NotNull(sources), comparer);
    }

    private static IEnumerable<TSource> MergeIterator<TSource>(IEnumerable<IEnumerable<TSource>> sources, Option<IComparer<TSource>> comparer)
    {
        // The enumerators are acquired inside the try, so the ones acquired before a failing GetEnumerator are disposed.
        var enumerators = new List<IEnumerator<TSource>>();

        try
        {
            foreach (var sequence in sources)
            {
                enumerators.Add(sequence.GetEnumerator());
            }

            foreach (var element in MergeEnumerators(enumerators.ToImmutableList().RemoveAll(HasMoreElements), GetMergeComparer(comparer)))
            {
                yield return element;
            }
        }
        finally
        {
            enumerators.ForEach(enumerator => enumerator.Dispose());
        }
    }

    private static IComparer<TSource> GetMergeComparer<TSource>(Option<IComparer<TSource>> comparer = default)
        => comparer.GetOrElse(Comparer<TSource>.Default);

    private static bool HasMoreElements<TSource>(IEnumerator<TSource> enumerator)
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

    private static ImmutableList<IEnumerator<TSource>> RemoveYielded<TSource>(IEnumerator<TSource> minimum, ImmutableList<IEnumerator<TSource>> enumerators)
        => minimum.MoveNext()
            ? enumerators
            : enumerators.Remove(minimum);

    private static IEnumerator<TSource> FindMinimum<TSource>(ImmutableList<IEnumerator<TSource>> enumerators, IComparer<TSource> comparer)
        => enumerators.Aggregate(Minimum(comparer));

    private static Func<IEnumerator<TSource>, IEnumerator<TSource>, IEnumerator<TSource>> Minimum<TSource>(IComparer<TSource> comparer)
        => (minimum, enumerator) => comparer.Compare(minimum.Current, enumerator.Current) <= 0 ? minimum : enumerator;
}
