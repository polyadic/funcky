#if INTEGRATED_ASYNC
using System.Runtime.CompilerServices;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
    /// </summary>
    /// <param name="left">The left sequence to merge.</param>
    /// <typeparam name="TLeft">Type of the elements in <paramref name="left"/> sequence.</typeparam>
    /// <param name="right">The right sequence to merge.</param>
    /// <typeparam name="TRight">Type of the elements in <paramref name="right"/> sequence.</typeparam>
    /// <returns>A sequence that contains merged elements of two input sequences.</returns>
    [Pure]
    public static IAsyncEnumerable<EitherOrBoth<TLeft, TRight>> ZipLongest<TLeft, TRight>(this IAsyncEnumerable<TLeft> left, IAsyncEnumerable<TRight> right)
        where TLeft : notnull
        where TRight : notnull
        => left.ZipLongest(right, Identity);

    /// <summary>
    /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
    /// </summary>
    /// <param name="left">The left sequence to merge.</param>
    /// <typeparam name="TLeft">Type of the elements in <paramref name="left"/> sequence.</typeparam>
    /// <param name="right">The right sequence to merge.</param>
    /// <typeparam name="TRight">Type of the elements in <paramref name="right"/> sequence.</typeparam>
    /// <typeparam name="TResult">The return type of the result selector function.</typeparam>
    /// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
    /// <returns>A sequence that contains merged elements of two input sequences.</returns>
    [Pure]
    public static async IAsyncEnumerable<TResult> ZipLongest<TLeft, TRight, TResult>(this IAsyncEnumerable<TLeft> left, IAsyncEnumerable<TRight> right, Func<EitherOrBoth<TLeft, TRight>, TResult> resultSelector)
        where TLeft : notnull
        where TRight : notnull
    {
        #pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var leftEnumerator = left.ConfigureAwait(false).GetAsyncEnumerator();
        await using var rightEnumerator = right.ConfigureAwait(false).GetAsyncEnumerator();
        #pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        while ((await MoveNextOrNone(leftEnumerator, rightEnumerator).ConfigureAwait(false)).TryGetValue(out var next))
        {
            yield return resultSelector(next);
        }
    }

    private static async ValueTask<Option<EitherOrBoth<TLeft, TRight>>> MoveNextOrNone<TLeft, TRight>(ConfiguredCancelableAsyncEnumerable<TLeft>.Enumerator leftEnumerator, ConfiguredCancelableAsyncEnumerable<TRight>.Enumerator rightEnumerator)
        where TLeft : notnull
        where TRight : notnull
        => EitherOrBoth.FromOptions(await ReadNext(leftEnumerator).ConfigureAwait(false), await ReadNext(rightEnumerator).ConfigureAwait(false));

    private static async ValueTask<Option<TSource>> ReadNext<TSource>(ConfiguredCancelableAsyncEnumerable<TSource>.Enumerator enumerator)
        where TSource : notnull
        => await enumerator.MoveNextAsync()
            ? enumerator.Current
            : Option<TSource>.None;
}
#endif
