using System.Collections.Immutable;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <param name="source">The source sequence.</param>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// Splits the source sequence by the given separator.
        /// </summary>
        /// <param name="separator">A single element of type <typeparamref name="TSource"/> separating the parts.</param>
        /// <returns>A sequence of sequences.</returns>
        [Pure]
        public IEnumerable<IReadOnlyList<TSource>> Split(TSource separator)
            => source.Split(separator, EqualityComparer<TSource>.Default);

        /// <summary>
        /// Splits the source sequence by the given separator and the given equality.
        /// </summary>
        /// <param name="separator">A single element of type <typeparamref name="TSource"/> separating the parts.</param>
        /// <param name="comparer">Override the default equality comparer.</param>
        /// <returns>A sequence of sequences.</returns>
        [Pure]
        public IEnumerable<IReadOnlyList<TSource>> Split(
            TSource separator,
            IEqualityComparer<TSource> comparer)
            => source.Split(separator, comparer, Identity);

        /// <summary>
        /// Splits the source sequence by the given separator and the given equality.
        /// With the resultSelector you can chose what to construct from the individual part.
        /// </summary>
        /// <typeparam name="TResult">Type of the elements produced by the <paramref name="resultSelector"/>.</typeparam>
        /// <param name="separator">A single element of type <typeparamref name="TSource"/> separating the parts.</param>
        /// <param name="comparer">Override the default equality comparer.</param>
        /// <param name="resultSelector">The result selector produces a result from each partial sequence.</param>
        /// <returns>A sequence of results.</returns>
        [Pure]
        public IEnumerable<TResult> Split<TResult>(
            TSource separator,
            IEqualityComparer<TSource> comparer,
            Func<IReadOnlyList<TSource>, TResult> resultSelector)
            => SplitIterator(ArgumentValidator.NotNull(source), separator, ArgumentValidator.NotNull(comparer), ArgumentValidator.NotNull(resultSelector));
    }

    /// <param name="source">The source array.</param>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> array.</typeparam>
    extension<TSource>(TSource[] source)
    {
        /// <summary>
        /// Splits the source array by the given separator.
        /// </summary>
        /// <remarks>
        /// This overload exists so that arrays keep binding to Funcky's <c>Split</c> in C# 14 and later, where
        /// the first-class span conversion would otherwise make <c>MemoryExtensions.Split(ReadOnlySpan{T}, T)</c> win.
        /// </remarks>
        /// <param name="separator">A single element of type <typeparamref name="TSource"/> separating the parts.</param>
        /// <returns>A sequence of sequences.</returns>
        [Pure]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public IEnumerable<IReadOnlyList<TSource>> Split(TSource separator)
            => source.Split(separator, EqualityComparer<TSource>.Default);
    }

    private static IEnumerable<TResult> SplitIterator<TSource, TResult>(
        IEnumerable<TSource> source,
        TSource separator,
        IEqualityComparer<TSource> comparer,
        Func<IReadOnlyList<TSource>, TResult> resultSelector)
    {
        var part = ImmutableList.CreateBuilder<TSource>();

        foreach (var element in source)
        {
            if (comparer.Equals(element, separator))
            {
                yield return resultSelector(part.ToImmutable());
                part.Clear();
            }
            else
            {
                part.Add(element);
            }
        }

        yield return resultSelector(part.ToImmutable());
    }
}
