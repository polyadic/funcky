#if INTEGRATED_ASYNC
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Groups adjacent elements of a source sequence with same key specified by the key selector function.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>An <see cref="IEnumerable{T}" /> where each element is an <see cref ="IGrouping{TKey,TElement}" /> object containing a sequence of objects and a key.</returns>
    [Pure]
    public static IAsyncEnumerable<IGrouping<TKey, TSource>> AdjacentGroupBy<TSource, TKey>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
        => AdjacentGroupByInternal(source, keySelector, Identity, CreateGrouping, EqualityComparer<TKey>.Default);

    /// <summary>
    /// Groups adjacent elements of a source sequence according to a specified key selector function and compares the keys by using a specified comparer.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>An <see cref="IEnumerable{T}" /> where each element is an <see cref ="IGrouping{TKey,TElement}" /> object containing a sequence of objects and a key.</returns>
    [Pure]
    public static IAsyncEnumerable<IGrouping<TKey, TSource>> AdjacentGroupBy<TSource, TKey>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey> comparer)
        => AdjacentGroupByInternal(source, keySelector, Identity, CreateGrouping, comparer);

    /// <summary>
    /// Groups adjacent elements of a source sequence according to a key selector function. The keys are compared by using a comparer and each group's elements are projected by using a specified function.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <typeparam name="TElement">The type of the elements in each <see cref ="IGrouping{TKey,TElement}" />.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="elementSelector">A function to map each source element to an element in the <see cref ="IGrouping{TKey,TElement}" />.</param>
    /// <returns>An <see cref="IEnumerable{T}" /> where each element is an <see cref ="IGrouping{TKey,TElement}" /> object containing a sequence of objects and a key.</returns>
    [Pure]
    public static IAsyncEnumerable<IGrouping<TKey, TElement>> AdjacentGroupBy<TSource, TKey, TElement>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TElement> elementSelector)
        => AdjacentGroupByInternal(source, keySelector, elementSelector, CreateGrouping, EqualityComparer<TKey>.Default);

    /// <summary>
    /// Groups adjacent elements of a source sequence according to a specified key selector function and projects the elements for each group by using a specified function.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <typeparam name="TElement">The type of the elements in each <see cref ="IGrouping{TKey,TElement}" />.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="elementSelector">A function to map each source element to an element in the <see cref ="IGrouping{TKey,TElement}" />.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>An <see cref="IEnumerable{T}" /> where each element is an <see cref ="IGrouping{TKey,TElement}" /> object containing a sequence of objects and a key.</returns>
    [Pure]
    public static IAsyncEnumerable<IGrouping<TKey, TElement>> AdjacentGroupBy<TSource, TKey, TElement>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TElement> elementSelector,
        IEqualityComparer<TKey> comparer)
        => AdjacentGroupByInternal(source, keySelector, elementSelector, CreateGrouping, comparer);

    /// <summary>
    /// Groups adjacent elements of a source sequence according to a specified key selector function and creates a result value from each group and its key.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="resultSelector">A function to map each source element to an element in the <see cref ="IGrouping{TKey,TElement}" />.</param>
    /// <returns>A collection of elements of type TResult where each element represents a projection over a group and its key.</returns>
    [Pure]
    public static IAsyncEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
        => AdjacentGroupByInternal(source, keySelector, Identity, resultSelector, EqualityComparer<TKey>.Default);

    /// <summary>
    /// Groups adjacent elements of a source sequence according to a specified key selector function and creates a result value from each group and its key. The elements of each group are projected by using a specified function.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <typeparam name="TElement">The type of the elements in each <see cref ="IGrouping{TKey,TElement}" />.</typeparam>
    /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="elementSelector">A function to map each source element to an element in the <see cref ="IGrouping{TKey,TElement}" />.</param>
    /// <param name="resultSelector">A function to create a result value from each group.</param>
    /// <returns>A collection of elements of type TResult where each element represents a projection over a group and its key.</returns>
    [Pure]
    public static IAsyncEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TElement, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TElement> elementSelector,
        Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        => AdjacentGroupByInternal(source, keySelector, elementSelector, resultSelector, EqualityComparer<TKey>.Default);

    /// <summary>
    /// Groups adjacent elements of a source sequence according to a specified key selector function and creates a result value from each group and its key. The keys are compared by using a specified comparer.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="resultSelector">A function to create a result value from each group.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>A collection of elements of type TResult where each element represents a projection over a group and its key.</returns>
    [Pure]
    public static IAsyncEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TKey, IEnumerable<TSource>, TResult> resultSelector,
        IEqualityComparer<TKey> comparer)
        => AdjacentGroupByInternal(source, keySelector, Identity, resultSelector, comparer);

    /// <summary>
    /// Groups adjacent elements of a source sequence according to a specified key selector function and creates a result value from each group and its key. Key values are compared by using a specified comparer, and the elements of each group are projected by using a specified function.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <typeparam name="TElement">The type of the elements in each <see cref ="IGrouping{TKey,TElement}" />.</typeparam>
    /// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="elementSelector">A function to map each source element to an element in the <see cref ="IGrouping{TKey,TElement}" />.</param>
    /// <param name="resultSelector">A function to create a result value from each group.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>A collection of elements of type TResult where each element represents a projection over a group and its key.</returns>
    [Pure]
    public static IAsyncEnumerable<TResult> AdjacentGroupBy<TSource, TKey, TElement, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TElement> elementSelector,
        Func<TKey, IImmutableList<TElement>, TResult> resultSelector,
        IEqualityComparer<TKey> comparer)
        => AdjacentGroupByInternal(source, keySelector, elementSelector, resultSelector, comparer);

    private static async IAsyncEnumerable<TResult> AdjacentGroupByInternal<TSource, TKey, TElement, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TElement> elementSelector,
        Func<TKey, IImmutableList<TElement>, TResult> resultSelector,
        IEqualityComparer<TKey> comparer,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var asyncEnumerator = source.GetAsyncEnumerator(cancellationToken);
        await using var sourceEnumerator = asyncEnumerator.ConfigureAwait(false);

        if (!await asyncEnumerator.MoveNextAsync().ConfigureAwait(false))
        {
            yield break;
        }

        var (group, key) = CreateGroupAndKey(keySelector, elementSelector, asyncEnumerator);

        while (await asyncEnumerator.MoveNextAsync().ConfigureAwait(false))
        {
            if (comparer.Equals(key, keySelector(asyncEnumerator.Current)))
            {
                group = group.Add(elementSelector(asyncEnumerator.Current));
            }
            else
            {
                yield return resultSelector(key, group);
                (group, key) = CreateGroupAndKey(keySelector, elementSelector, asyncEnumerator);
            }
        }

        yield return resultSelector(key, group);
    }

    private static Grouping<TKey, TElement> CreateGrouping<TKey, TElement>(TKey key, IImmutableList<TElement> elements)
        => new(key, elements);

    private static (IImmutableList<TElement> Group, TKey Key) CreateGroupAndKey<TSource, TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IAsyncEnumerator<TSource> enumerator)
    {
        var group = ImmutableList.Create(elementSelector(enumerator.Current));
        var key = keySelector(enumerator.Current);

        return (group, key);
    }
}
#endif
