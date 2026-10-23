#if ASYNC_SUPPORTED
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <param name="source">The source sequence.</param>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>
        /// Splits the source sequence by the given separator.
        /// </summary>
        /// <param name="separator">A single element of type <typeparamref name="TSource"/> separating the parts.</param>
        /// <returns>A sequence of sequences.</returns>
        [Pure]
        public IAsyncEnumerable<IReadOnlyList<TSource>> Split(TSource separator)
            => SplitEnumerator(ArgumentValidator.NotNull(source), separator, EqualityComparer<TSource>.Default);

        /// <summary>
        /// Splits the source sequence by the given separator and the given equality.
        /// </summary>
        /// <param name="separator">A single element of type <typeparamref name="TSource"/> separating the parts.</param>
        /// <param name="comparer">Override the default equality comparer.</param>
        /// <returns>A sequence of sequences.</returns>
        [Pure]
        public IAsyncEnumerable<IReadOnlyList<TSource>> Split(TSource separator, IEqualityComparer<TSource> comparer)
            => SplitEnumerator(ArgumentValidator.NotNull(source), separator, ArgumentValidator.NotNull(comparer));
    }

    /// <param name="source">The source sequence.</param>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    extension<TSource>(IAsyncEnumerable<TSource> source)
        where TSource : notnull
    {
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
        public IAsyncEnumerable<TResult> Split<TResult>(
            TSource separator,
            IEqualityComparer<TSource> comparer,
            Func<IReadOnlyList<TSource>, TResult> resultSelector)
            => SplitEnumerator(ArgumentValidator.NotNull(source), separator, ArgumentValidator.NotNull(comparer))
                .Select(ArgumentValidator.NotNull(resultSelector));
    }

    private static async IAsyncEnumerable<IReadOnlyList<TSource>> SplitEnumerator<TSource>(IAsyncEnumerable<TSource> source, TSource separator, IEqualityComparer<TSource> comparer, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var part = ImmutableList.CreateBuilder<TSource>();

        await foreach (var element in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (comparer.Equals(element, separator))
            {
                yield return part.ToImmutable();
                part.Clear();
            }
            else
            {
                part.Add(element);
            }
        }

        yield return part.ToImmutable();
    }
}
#endif
