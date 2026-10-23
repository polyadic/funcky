#if ASYNC_SUPPORTED
using System.Runtime.CompilerServices;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        /// <summary>Returns the items from <paramref name="source"/> when it has any or otherwise the items from <paramref name="fallback"/>.</summary>
        [Pure]
        public IAsyncEnumerable<TSource> AnyOrElse(IAsyncEnumerable<TSource> fallback)
            => AnyOrElseInternal(ArgumentValidator.NotNull(source), FallbackProvider(ArgumentValidator.NotNull(fallback)));

        /// <summary>Returns the items from <paramref name="source"/> when it has any or otherwise the items from <paramref name="fallback"/>.</summary>
        [Pure]
        public IAsyncEnumerable<TSource> AnyOrElse(Func<IAsyncEnumerable<TSource>> fallback)
            => AnyOrElseInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(fallback));
    }

    private static Func<IAsyncEnumerable<TSource>> FallbackProvider<TSource>(IAsyncEnumerable<TSource> fallback)
        => () => fallback;

    private static async IAsyncEnumerable<TSource> AnyOrElseInternal<TSource>(IAsyncEnumerable<TSource> source, Func<IAsyncEnumerable<TSource>> fallback, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var hasItems = false;

        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            hasItems = true;
            yield return item;
        }

        if (!hasItems)
        {
            await foreach (var item in fallback().WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return item;
            }
        }
    }
}
#endif
