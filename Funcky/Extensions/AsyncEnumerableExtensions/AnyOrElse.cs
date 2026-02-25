#if INTEGRATED_ASYNC
using System.Runtime.CompilerServices;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>Returns the items from <paramref name="source"/> when it has any or otherwise the items from <paramref name="fallback"/>.</summary>
    [Pure]
    public static IAsyncEnumerable<TSource> AnyOrElse<TSource>(this IAsyncEnumerable<TSource> source, IAsyncEnumerable<TSource> fallback)
        => AnyOrElseInternal(source, () => fallback);

    /// <summary>Returns the items from <paramref name="source"/> when it has any or otherwise the items from <paramref name="fallback"/>.</summary>
    [Pure]
    public static IAsyncEnumerable<TSource> AnyOrElse<TSource>(this IAsyncEnumerable<TSource> source, Func<IAsyncEnumerable<TSource>> fallback)
        => AnyOrElseInternal(source, fallback);

    private static async IAsyncEnumerable<TSource> AnyOrElseInternal<TSource>(IAsyncEnumerable<TSource> source, Func<IAsyncEnumerable<TSource>> fallback, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var hasItems = false;

        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            hasItems = true;
            yield return item;
        }

        if (!hasItems)
        {
            await foreach (var item in fallback().WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }
    }
}
#endif
