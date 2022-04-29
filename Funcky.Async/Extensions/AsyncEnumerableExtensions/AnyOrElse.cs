using System.Runtime.CompilerServices;

namespace Funcky.Async.Extensions;

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
        var asyncEnumerator = source.GetAsyncEnumerator(cancellationToken);
        var sourceEnumerator = asyncEnumerator.ConfigureAwait(false);

        if (await asyncEnumerator.MoveNextAsync().ConfigureAwait(false))
        {
            yield return asyncEnumerator.Current;
        }
        else
        {
            asyncEnumerator = fallback().GetAsyncEnumerator(cancellationToken);
            await sourceEnumerator.DisposeAsync();
            sourceEnumerator = asyncEnumerator.ConfigureAwait(false);
        }

        while (await asyncEnumerator.MoveNextAsync().ConfigureAwait(false))
        {
            yield return asyncEnumerator.Current;
        }

        await sourceEnumerator.DisposeAsync();
    }
}
