#if INTEGRATED_ASYNC
using System.Runtime.CompilerServices;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// An IAsyncEnumerable that calls a function if and only if the source has no element to enumerate. It can be used to encode side effects on an empty IAsyncEnumerable.
    /// The side effect will be executed when enumerating the result.
    /// </summary>
    /// <typeparam name="TSource">the inner type of the enumerable.</typeparam>
    /// <returns>returns an <see cref="IAsyncEnumerable{T}" /> with the side effect defined by action encoded in the async enumerable.</returns>
    [Pure]
    public static IAsyncEnumerable<TSource> InspectEmpty<TSource>(this IAsyncEnumerable<TSource> source, Action inspector)
        => InspectEmptyInternal(source, inspector);

    private static async IAsyncEnumerable<TSource> InspectEmptyInternal<TSource>(
        this IAsyncEnumerable<TSource> source,
        Action inspector,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var enumerator = source.ConfigureAwait(false).WithCancellation(cancellationToken).GetAsyncEnumerator();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        if (await enumerator.MoveNextAsync())
        {
            yield return enumerator.Current;
        }
        else
        {
            inspector();
            yield break;
        }

        while (await enumerator.MoveNextAsync())
        {
            yield return enumerator.Current;
        }
    }
}
#endif
