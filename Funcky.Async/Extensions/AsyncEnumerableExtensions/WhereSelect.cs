namespace Funcky.Async.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Filters out all the empty values from an <c>IEnumerable&lt;Option&lt;T&gt;&gt;</c> and therefore returns an <see cref="IEnumerable{TItem}"/>.
    /// </summary>
    [Pure]
    public static IAsyncEnumerable<TItem> WhereSelect<TItem>(this IAsyncEnumerable<Option<TItem>> source)
        where TItem : notnull
        => source.WhereSelect(Identity);

    /// <summary>
    /// Projects and filters an <see cref="IAsyncEnumerable{T}"/> at the same time.
    /// This is done by filtering out any empty <see cref="Option{T}"/> values returned by the <paramref name="selector"/>.
    /// </summary>
    [Pure]
    public static IAsyncEnumerable<TOutput> WhereSelect<TSource, TOutput>(this IAsyncEnumerable<TSource> source, Func<TSource, Option<TOutput>> selector)
        where TOutput : notnull
        => source.Select(selector).SelectMany(ToAsyncEnumerable);

    /// <inheritdoc cref="WhereSelect{TSource,TOutput}"/>
    [Pure]
    public static IAsyncEnumerable<TOutput> WhereSelectAwait<TSource, TOutput>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<Option<TOutput>>> selector)
        where TOutput : notnull
        => source.SelectAwait(selector).SelectMany(ToAsyncEnumerable);

    /// <inheritdoc cref="WhereSelect{TSource,TOutput}"/>
    [Pure]
    public static IAsyncEnumerable<TOutput> WhereSelectAwaitWithCancellation<TSource, TOutput>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<Option<TOutput>>> selector)
        where TOutput : notnull
        => source.SelectAwaitWithCancellation(selector).SelectMany(ToAsyncEnumerable);

    private static IAsyncEnumerable<TItem> ToAsyncEnumerable<TItem>(Option<TItem> option)
        where TItem : notnull
        => option.Match(
            none: AsyncEnumerable.Empty<TItem>,
            some: AsyncSequence.Return);
}
