using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>Returns the items from <paramref name="source"/> when it has any or otherwise the items from <paramref name="fallback"/>.</summary>
        [Pure]
        public IEnumerable<TSource> AnyOrElse(IEnumerable<TSource> fallback)
            => AnyOrElseUnchecked(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(fallback));

        /// <summary>Returns the items from <paramref name="source"/> when it has any or otherwise the items from <paramref name="fallback"/>.</summary>
        /// <remarks>The <paramref name="fallback"/> is invoked when the result is enumerated, not when this method is called.</remarks>
        [Pure]
        public IEnumerable<TSource> AnyOrElse(Func<IEnumerable<TSource>> fallback)
            => AnyOrElseUnchecked(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(fallback));
    }

    private static IEnumerable<TSource> AnyOrElseUnchecked<TSource>(IEnumerable<TSource> source, IEnumerable<TSource> fallback)
        => source switch
        {
            _ when source.TryGetNonEnumeratedCount(out var count) => count > 0 ? source : fallback,
            _ => AnyOrElseInternal(source, FallbackProvider(fallback)),
        };

    private static IEnumerable<TSource> AnyOrElseUnchecked<TSource>(IEnumerable<TSource> source, Func<IEnumerable<TSource>> fallback)
        => source switch
        {
            _ when source.TryGetNonEnumeratedCount(out var count) => count > 0 ? source : DeferredFallback(fallback),
            _ => AnyOrElseInternal(source, fallback),
        };

    private static Func<IEnumerable<TSource>> FallbackProvider<TSource>(IEnumerable<TSource> fallback)
        => () => fallback;

    private static IEnumerable<TSource> DeferredFallback<TSource>(Func<IEnumerable<TSource>> fallback)
    {
        foreach (var item in fallback())
        {
            yield return item;
        }
    }

    private static IEnumerable<TSource> AnyOrElseInternal<TSource>(IEnumerable<TSource> source, Func<IEnumerable<TSource>> fallback)
    {
        var hasItems = false;

        foreach (var item in source)
        {
            hasItems = true;
            yield return item;
        }

        if (!hasItems)
        {
            foreach (var item in fallback())
            {
                yield return item;
            }
        }
    }
}
