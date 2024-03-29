using System.Runtime.CompilerServices;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Splits the source sequence by the given separator.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="separator">A single element of type <typeparamref name="TSource"/> separating the parts.</param>
    /// <returns>A sequence of sequences.</returns>
    [Pure]
    public static IAsyncEnumerable<IReadOnlyList<TSource>> Split<TSource>(this IAsyncEnumerable<TSource> source, TSource separator)
        => SplitEnumerator(source, separator, EqualityComparer<TSource>.Default);

    /// <summary>
    /// Splits the source sequence by the given separator and the given equality.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="separator">A single element of type <typeparamref name="TSource"/> separating the parts.</param>
    /// <param name="comparer">Override the default equality comparer.</param>
    /// <returns>A sequence of sequences.</returns>
    [Pure]
    public static IAsyncEnumerable<IReadOnlyList<TSource>> Split<TSource>(this IAsyncEnumerable<TSource> source, TSource separator, IEqualityComparer<TSource> comparer)
        => SplitEnumerator(source, separator, comparer);

    /// <summary>
    /// Splits the source sequence by the given separator and the given equality.
    /// With the resultSelector you can chose what to construct from the individual part.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TResult">Type of the elements produced by the <paramref name="resultSelector"/>.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="separator">A single element of type <typeparamref name="TSource"/> separating the parts.</param>
    /// <param name="comparer">Override the default equality comparer.</param>
    /// <param name="resultSelector">The result selector produces a result from each partial sequence.</param>
    /// <returns>A sequence of results.</returns>
    [Pure]
    public static IAsyncEnumerable<TResult> Split<TSource, TResult>(
        this IAsyncEnumerable<TSource> source,
        TSource separator,
        IEqualityComparer<TSource> comparer,
        Func<IReadOnlyList<TSource>, TResult> resultSelector)
        where TSource : notnull
        => SplitEnumerator(source, separator, comparer)
            .Select(resultSelector);

    private static async IAsyncEnumerable<IReadOnlyList<TSource>> SplitEnumerator<TSource>(IAsyncEnumerable<TSource> source, TSource separator, IEqualityComparer<TSource> comparer, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var asyncEnumerator = source.GetAsyncEnumerator(cancellationToken);
        await using var sourceEnumerator = asyncEnumerator.ConfigureAwait(false);

        while (await asyncEnumerator.MoveNextAsync().ConfigureAwait(false))
        {
            yield return await TakeSkipWhile(asyncEnumerator, TakeSkipPredicate(separator, comparer)).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private static Func<TSource, bool> TakeSkipPredicate<TSource>(TSource separator, IEqualityComparer<TSource> comparer)
        => element
            => !comparer.Equals(element, separator);

    private static async IAsyncEnumerable<TSource> TakeSkipWhile<TSource>(IAsyncEnumerator<TSource> source, Func<TSource, bool> predicate)
    {
        do
        {
            if (predicate(source.Current))
            {
                yield return source.Current;
            }
            else
            {
                yield break;
            }
        }
        while (await source.MoveNextAsync().ConfigureAwait(false));
    }
}
