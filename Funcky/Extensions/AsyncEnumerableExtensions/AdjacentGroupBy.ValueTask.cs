#if ASYNC_SUPPORTED
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Funcky.Internal.Validators;
using static Funcky.ValueTaskFactory;

namespace Funcky.Extensions;

// The overloads of AdjacentGroupBy with asynchronous selectors. Each selector receives the cancellation token of the
// enumeration, the one shape the base class library uses for asynchronous selectors since .NET 10.
public static partial class AsyncEnumerableExtensions
{
    /// <inheritdoc cref="AdjacentGroupBy{TSource, TKey}(IAsyncEnumerable{TSource}, Func{TSource, TKey})"/>
    [Pure]
    [OverloadResolutionPriority(1)]
    public static IAsyncEnumerable<IGrouping<TKey, TSource>> AdjacentGroupBy<TSource, TKey>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector)
        => AdjacentGroupByAwaitWithCancellationInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(keySelector), ValueTaskFromResult, CreateGroupingAwait, EqualityComparer<TKey>.Default);

    /// <inheritdoc cref="AdjacentGroupBy{TSource, TKey}(IAsyncEnumerable{TSource}, Func{TSource, TKey}, IEqualityComparer{TKey})"/>
    [Pure]
    [OverloadResolutionPriority(1)]
    public static IAsyncEnumerable<IGrouping<TKey, TSource>> AdjacentGroupBy<TSource, TKey>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        IEqualityComparer<TKey> comparer)
        => AdjacentGroupByAwaitWithCancellationInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(keySelector), ValueTaskFromResult, CreateGroupingAwait, ArgumentValidator.NotNull(comparer));

    /// <inheritdoc cref="AdjacentGroupBy{TSource, TKey, TElement}(IAsyncEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement})"/>
    [Pure]
    [OverloadResolutionPriority(1)]
    public static IAsyncEnumerable<IGrouping<TKey, TElement>> AdjacentGroupBy<TSource, TKey, TElement>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector)
        => AdjacentGroupByAwaitWithCancellationInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(keySelector), ArgumentValidator.NotNull(elementSelector), CreateGroupingAwait, EqualityComparer<TKey>.Default);

    /// <inheritdoc cref="AdjacentGroupBy{TSource, TKey, TElement}(IAsyncEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement}, IEqualityComparer{TKey})"/>
    [Pure]
    [OverloadResolutionPriority(1)]
    public static IAsyncEnumerable<IGrouping<TKey, TElement>> AdjacentGroupBy<TSource, TKey, TElement>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector,
        IEqualityComparer<TKey> comparer)
        => AdjacentGroupByAwaitWithCancellationInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(keySelector), ArgumentValidator.NotNull(elementSelector), CreateGroupingAwait, ArgumentValidator.NotNull(comparer));

    /// <inheritdoc cref="AdjacentGroupBy{TSource, TKey, TResult}(IAsyncEnumerable{TSource}, Func{TSource, TKey}, Func{TKey, IEnumerable{TSource}, TResult})"/>
    [Pure]
    [OverloadResolutionPriority(1)]
    public static IAsyncEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        Func<TKey, IEnumerable<TSource>, CancellationToken, ValueTask<TResult>> resultSelector)
        => AdjacentGroupByAwaitWithCancellationInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(keySelector), ValueTaskFromResult, ArgumentValidator.NotNull(resultSelector), EqualityComparer<TKey>.Default);

    /// <inheritdoc cref="AdjacentGroupBy{TSource, TKey, TElement, TResult}(IAsyncEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement}, Func{TKey, IEnumerable{TElement}, TResult})"/>
    [Pure]
    [OverloadResolutionPriority(1)]
    public static IAsyncEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TElement, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector,
        Func<TKey, IEnumerable<TElement>, CancellationToken, ValueTask<TResult>> resultSelector)
        => AdjacentGroupByAwaitWithCancellationInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(keySelector), ArgumentValidator.NotNull(elementSelector), ArgumentValidator.NotNull(resultSelector), EqualityComparer<TKey>.Default);

    /// <inheritdoc cref="AdjacentGroupBy{TSource, TKey, TResult}(IAsyncEnumerable{TSource}, Func{TSource, TKey}, Func{TKey, IEnumerable{TSource}, TResult}, IEqualityComparer{TKey})"/>
    [Pure]
    [OverloadResolutionPriority(1)]
    public static IAsyncEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        Func<TKey, IEnumerable<TSource>, CancellationToken, ValueTask<TResult>> resultSelector,
        IEqualityComparer<TKey> comparer)
        => AdjacentGroupByAwaitWithCancellationInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(keySelector), ValueTaskFromResult, ArgumentValidator.NotNull(resultSelector), ArgumentValidator.NotNull(comparer));

    /// <inheritdoc cref="AdjacentGroupBy{TSource, TKey, TElement, TResult}(IAsyncEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement}, Func{TKey, IImmutableList{TElement}, TResult}, IEqualityComparer{TKey})"/>
    [Pure]
    [OverloadResolutionPriority(1)]
    public static IAsyncEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TElement, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector,
        Func<TKey, IImmutableList<TElement>, CancellationToken, ValueTask<TResult>> resultSelector,
        IEqualityComparer<TKey> comparer)
        => AdjacentGroupByAwaitWithCancellationInternal(ArgumentValidator.NotNull(source), ArgumentValidator.NotNull(keySelector), ArgumentValidator.NotNull(elementSelector), ArgumentValidator.NotNull(resultSelector), ArgumentValidator.NotNull(comparer));

    private static async IAsyncEnumerable<TResult> AdjacentGroupByAwaitWithCancellationInternal<TSource, TKey, TElement, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector,
        Func<TKey, IImmutableList<TElement>, CancellationToken, ValueTask<TResult>> resultSelector,
        IEqualityComparer<TKey> comparer,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var asyncEnumerator = source.GetAsyncEnumerator(cancellationToken);
        await using var sourceEnumerator = asyncEnumerator.ConfigureAwait(false);

        if (!await asyncEnumerator.MoveNextAsync().ConfigureAwait(false))
        {
            yield break;
        }

        var (group, key) = await CreateGroupAndKeyAsync(keySelector, elementSelector, asyncEnumerator, cancellationToken).ConfigureAwait(false);

        while (await asyncEnumerator.MoveNextAsync().ConfigureAwait(false))
        {
            if (comparer.Equals(key, await keySelector(asyncEnumerator.Current, cancellationToken).ConfigureAwait(false)))
            {
                group = group.Add(await elementSelector(asyncEnumerator.Current, cancellationToken).ConfigureAwait(false));
            }
            else
            {
                yield return await resultSelector(key, group, cancellationToken).ConfigureAwait(false);
                (group, key) = await CreateGroupAndKeyAsync(keySelector, elementSelector, asyncEnumerator, cancellationToken).ConfigureAwait(false);
            }
        }

        yield return await resultSelector(key, group, cancellationToken).ConfigureAwait(false);
    }

    private static ValueTask<Grouping<TKey, TElement>> CreateGroupingAwait<TKey, TElement>(
        TKey key,
        IImmutableList<TElement> elements,
        CancellationToken cancellationToken)
        => ValueTaskFromResult(new Grouping<TKey, TElement>(key, elements), cancellationToken);

    private static async Task<(IImmutableList<TElement> Group, TKey Key)> CreateGroupAndKeyAsync<TSource, TKey, TElement>(
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector,
        IAsyncEnumerator<TSource> enumerator,
        CancellationToken cancellationToken)
    {
        var group = ImmutableList.Create(await elementSelector(enumerator.Current, cancellationToken).ConfigureAwait(false));
        var key = await keySelector(enumerator.Current, cancellationToken).ConfigureAwait(false);

        return (group, key);
    }
}
#endif
