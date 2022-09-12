namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Filters out all the empty values from an <c>IEnumerable&lt;Option&lt;T&gt;&gt;</c> and therefore returns an <see cref="IEnumerable{T}"/>.
    /// </summary>
    [Pure]
    public static IAsyncEnumerable<TSource> WhereSelect<TSource>(this IAsyncEnumerable<Option<TSource>> source)
        where TSource : notnull
        => source.WhereSelect(Identity);

    /// <summary>
    /// Projects and filters an <see cref="IAsyncEnumerable{T}"/> at the same time.
    /// This is done by filtering out any empty <see cref="Option{T}"/> values returned by the <paramref name="selector"/>.
    /// </summary>
    [Pure]
    public static IAsyncEnumerable<TResult> WhereSelect<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, Option<TResult>> selector)
        where TResult : notnull
        => source.Select(selector).SelectMany(ToAsyncEnumerable);

    /// <inheritdoc cref="WhereSelect{TSource,TOutput}"/>
    [Pure]
    public static IAsyncEnumerable<TResult> WhereSelectAwait<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<Option<TResult>>> selector)
        where TResult : notnull
        => source.SelectAwait(selector).SelectMany(ToAsyncEnumerable);

    /// <inheritdoc cref="WhereSelect{TSource,TOutput}"/>
    [Pure]
    public static IAsyncEnumerable<TResult> WhereSelectAwaitWithCancellation<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<Option<TResult>>> selector)
        where TResult : notnull
        => source.SelectAwaitWithCancellation(selector).SelectMany(ToAsyncEnumerable);

    private static IAsyncEnumerable<TItem> ToAsyncEnumerable<TItem>(Option<TItem> option)
        where TItem : notnull
        => option.Match(
            none: AsyncEnumerable.Empty<TItem>,
            some: AsyncSequence.Return);
}
