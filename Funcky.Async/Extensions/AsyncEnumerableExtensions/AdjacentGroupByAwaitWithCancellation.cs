using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using static Funcky.Async.ValueTaskFactory;

namespace Funcky.Async.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Groups adjacent elements of a source sequence with same key specified by the key selector function.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>An <see cref="IEnumerable{T}" /> where each element is an <see cref ="IGrouping{TKey,TElement}" /> object containing a sequence of objects and a key.</returns>
    [Pure]
    public static IAsyncEnumerable<IAsyncGrouping<TKey, TSource>> AdjacentGroupByAwaitWithCancellation<TSource, TKey>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector)
        => AdjacentGroupByAwaitWithCancellationInternal(source, keySelector, ValueTaskFromResult, CreateGroupingAwait, EqualityComparer<TKey>.Default);

    /// <summary>
    /// Groups adjacent elements of a source sequence according to a specified key selector function and compares the keys by using a specified comparer.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">An IEqualityComparer{T} to compare keys.</param>
    /// <returns>An <see cref="IEnumerable{T}" /> where each element is an <see cref ="IGrouping{TKey,TElement}" /> object containing a sequence of objects and a key.</returns>
    [Pure]
    public static IAsyncEnumerable<IAsyncGrouping<TKey, TSource>> AdjacentGroupByAwaitWithCancellation<TSource, TKey>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        IEqualityComparer<TKey> comparer)
        => AdjacentGroupByAwaitWithCancellationInternal(source, keySelector, ValueTaskFromResult, CreateGroupingAwait, comparer);

    /// <summary>
    /// Groups adjacent elements of a source sequence according to a key selector function. The keys are compared by using a comparer and each group's elements are projected by using a specified function.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <typeparam name="TElement">The type of the elements in each <see cref ="IGrouping{TKey,TElement}" />.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="elementSelector">A function to map each source element to an element in the <see cref ="IGrouping{TKey,TElement}" />.</param>
    /// <returns>An <see cref="IEnumerable{T}" /> where each element is an <see cref ="IGrouping{TKey,TElement}" /> object containing a sequence of objects and a key.</returns>
    [Pure]
    public static IAsyncEnumerable<IAsyncGrouping<TKey, TElement>> AdjacentGroupByAwaitWithCancellation<TSource, TKey, TElement>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector)
        => AdjacentGroupByAwaitWithCancellationInternal(source, keySelector, elementSelector, CreateGroupingAwait, EqualityComparer<TKey>.Default);

    /// <summary>
    /// Groups adjacent elements of a source sequence according to a specified key selector function and projects the elements for each group by using a specified function.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <typeparam name="TElement">The type of the elements in each <see cref ="IGrouping{TKey,TElement}" />.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="elementSelector">A function to map each source element to an element in the <see cref ="IGrouping{TKey,TElement}" />.</param>
    /// <param name="comparer">An IEqualityComparer{T} to compare keys.</param>
    /// <returns>An <see cref="IEnumerable{T}" /> where each element is an <see cref ="IGrouping{TKey,TElement}" /> object containing a sequence of objects and a key.</returns>
    [Pure]
    public static IAsyncEnumerable<IAsyncGrouping<TKey, TElement>> AdjacentGroupByAwaitWithCancellation<TSource, TKey, TElement>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector,
        IEqualityComparer<TKey> comparer)
        => AdjacentGroupByAwaitWithCancellationInternal(source, keySelector, elementSelector, CreateGroupingAwait, comparer);

    /// <summary>
    /// Groups adjacent elements of a source sequence according to a specified key selector function and creates a result value from each group and its key.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <typeparam name="TResult">The type of the result value returned by resultSelector.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="resultSelector">A function to map each source element to an element in the <see cref ="IGrouping{TKey,TElement}" />.</param>
    /// <returns>A collection of elements of type TResult where each element represents a projection over a group and its key.</returns>
    [Pure]
    public static IAsyncEnumerable<TResult> AdjacentGroupByAwaitWithCancellation<TSource, TKey, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        Func<TKey, IEnumerable<TSource>, CancellationToken, ValueTask<TResult>> resultSelector)
        => AdjacentGroupByAwaitWithCancellationInternal(source, keySelector, ValueTaskFromResult, resultSelector, EqualityComparer<TKey>.Default);

    /// <summary>
    /// Groups adjacent elements of a source sequence according to a specified key selector function and creates a result value from each group and its key. The elements of each group are projected by using a specified function.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <typeparam name="TElement">The type of the elements in each <see cref ="IGrouping{TKey,TElement}" />.</typeparam>
    /// <typeparam name="TResult">The type of the result value returned by resultSelector.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="elementSelector">A function to map each source element to an element in the <see cref ="IGrouping{TKey,TElement}" />.</param>
    /// <param name="resultSelector">A function to create a result value from each group.</param>
    /// <returns>A collection of elements of type TResult where each element represents a projection over a group and its key.</returns>
    [Pure]
    public static IAsyncEnumerable<TResult> AdjacentGroupByAwaitWithCancellation<TSource, TKey, TElement, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector,
        Func<TKey, IEnumerable<TElement>, CancellationToken, ValueTask<TResult>> resultSelector)
        => AdjacentGroupByAwaitWithCancellationInternal(source, keySelector, elementSelector, resultSelector, EqualityComparer<TKey>.Default);

    /// <summary>
    /// Groups adjacent elements of a source sequence according to a specified key selector function and creates a result value from each group and its key. The keys are compared by using a specified comparer.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <typeparam name="TResult">The type of the result value returned by resultSelector.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="resultSelector">A function to create a result value from each group.</param>
    /// <param name="comparer">An IEqualityComparer{T} to compare keys.</param>
    /// <returns>A collection of elements of type TResult where each element represents a projection over a group and its key.</returns>
    [Pure]
    public static IAsyncEnumerable<TResult> AdjacentGroupByAwaitWithCancellation<TSource, TKey, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        Func<TKey, IEnumerable<TSource>, CancellationToken, ValueTask<TResult>> resultSelector,
        IEqualityComparer<TKey> comparer)
        => AdjacentGroupByAwaitWithCancellationInternal(source, keySelector, ValueTaskFromResult, resultSelector, comparer);

    /// <summary>
    /// Groups adjacent elements of a source sequence according to a specified key selector function and creates a result value from each group and its key. Key values are compared by using a specified comparer, and the elements of each group are projected by using a specified function.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <typeparam name="TElement">The type of the elements in each <see cref ="IGrouping{TKey,TElement}" />.</typeparam>
    /// <typeparam name="TResult">The type of the result value returned by resultSelector.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="elementSelector">A function to map each source element to an element in the <see cref ="IGrouping{TKey,TElement}" />.</param>
    /// <param name="resultSelector">A function to create a result value from each group.</param>
    /// <param name="comparer">An IEqualityComparer{T} to compare keys.</param>
    /// <returns>A collection of elements of type TResult where each element represents a projection over a group and its key.</returns>
    [Pure]
    public static IAsyncEnumerable<TResult> AdjacentGroupByAwaitWithCancellation<TSource, TKey, TElement, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, CancellationToken, ValueTask<TKey>> keySelector,
        Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector,
        Func<TKey, IImmutableList<TElement>, CancellationToken, ValueTask<TResult>> resultSelector,
        IEqualityComparer<TKey> comparer)
        => AdjacentGroupByAwaitWithCancellationInternal(source, keySelector, elementSelector, resultSelector, comparer);

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

    private static ValueTask<AsyncGrouping<TKey, TElement>> CreateGroupingAwait<TKey, TElement>(
        TKey key,
        IImmutableList<TElement> elements,
        CancellationToken cancellationToken)
        => ValueTaskFromResult(new AsyncGrouping<TKey, TElement>(key, elements));

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
