namespace Funcky;

public static partial class AsyncSequence
{
    /// <summary>
    /// Generates a sequence based on a <paramref name="successor"/> function stopping at the first <see cref="Option{TItem}.None"/> value.
    /// This is essentially the inverse operation of an <see cref="AsyncEnumerable.AggregateAsync{T}"/>.
    /// </summary>
    /// <param name="first">The first element of the sequence.</param>
    /// <param name="successor">Generates the next element of the sequence or <see cref="Option{TItem}.None"/> based on the previous item.</param>
    /// <remarks>Use <see cref="AsyncEnumerable.Skip{TSource}(IAsyncEnumerable{TSource}, int)"/> on the result if you don't want the first item to be included.</remarks>
    [Pure]
    public static async IAsyncEnumerable<TItem> Successors<TItem>(Option<TItem> first, Func<TItem, ValueTask<Option<TItem>>> successor)
        where TItem : notnull
    {
        var item = first;
        while (item.TryGetValue(out var itemValue))
        {
            yield return itemValue;
            item = await successor(itemValue).ConfigureAwait(false);
        }
    }

    /// <inheritdoc cref="Successors{TItem}(Option{TItem}, Func{TItem, ValueTask{Option{TItem}}})" />
    [Pure]
    public static IAsyncEnumerable<TItem> Successors<TItem>(TItem first, Func<TItem, ValueTask<Option<TItem>>> successor)
        where TItem : notnull
        => Successors(Option.Some(first), successor);

    /// <inheritdoc cref="Successors{TItem}(Option{TItem}, Func{TItem, ValueTask{Option{TItem}}})" />
    [Pure]
    public static IAsyncEnumerable<TItem> Successors<TItem>(Option<TItem> first, Func<TItem, ValueTask<TItem>> successor)
        where TItem : notnull
        => Successors(first, async previous => Option.Some(await successor(previous).ConfigureAwait(false)));

    /// <inheritdoc cref="Successors{TItem}(Option{TItem}, Func{TItem, ValueTask{Option{TItem}}})" />
    [Pure]
    public static IAsyncEnumerable<TItem> Successors<TItem>(TItem first, Func<TItem, ValueTask<TItem>> successor)
        where TItem : notnull
        => Successors(Option.Some(first), async previous => Option.Some(await successor(previous).ConfigureAwait(false)));
}
