namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Returns the element at a specified index in a sequence or an <see cref="Option{T}.None" /> value if the index is out of range.
    /// </summary>
    /// <typeparam name="TSource">The type of element contained by the sequence.</typeparam>
    /// <param name="source">The sequence to find an element in.</param>
    /// <param name="index">The index for the element to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The item at the specified index, or <see cref="Option{T}.None" /> if the index is not found.</returns>
    [Pure]
    public static async ValueTask<Option<TSource>> ElementAtOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, int index, CancellationToken cancellationToken = default)
        where TSource : notnull
        => await source.Select(Option.Some).ElementAtOrDefaultAsync(index, cancellationToken).ConfigureAwait(false);

#if INDEX_TYPE
    [Pure]
    public static ValueTask<Option<TSource>> ElementAtOrNoneAsync<TSource>(this IAsyncEnumerable<TSource> source, Index index, CancellationToken cancellationToken = default)
        where TSource : notnull
        => index.IsFromEnd
            ? ElementAtOrNoneFromEnd(source, index.Value, cancellationToken)
            : source.ElementAtOrNoneAsync(index.Value, cancellationToken);

    // Adopted from: https://github.com/dotnet/runtime/blob/ebba1d4acb7abea5ba15e1f7f69d1d1311465d16/src/libraries/System.Linq/src/System/Linq/ElementAt.cs#L152-L183
    private static async ValueTask<Option<TSource>> ElementAtOrNoneFromEnd<TSource>(IAsyncEnumerable<TSource> source, int indexFromEnd, CancellationToken cancellationToken)
        where TSource : notnull
    {
        if (indexFromEnd > 0)
        {
#pragma warning disable CA2007
            await using var enumerator = source.ConfigureAwait(false).WithCancellation(cancellationToken).GetAsyncEnumerator();
#pragma warning restore CA2007

            if (await enumerator.MoveNextAsync())
            {
                var queue = new Queue<TSource>();
                queue.Enqueue(enumerator.Current);

                while (await enumerator.MoveNextAsync())
                {
                    if (queue.Count == indexFromEnd)
                    {
                        queue.Dequeue();
                    }

                    queue.Enqueue(enumerator.Current);
                }

                if (queue.Count == indexFromEnd)
                {
                    return queue.Dequeue();
                }
            }
        }

        return Option<TSource>.None;
    }
#endif
}
