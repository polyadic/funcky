#if INTEGRATED_ASYNC
namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Filters out all the empty values from an <see cref="IAsyncEnumerable{T}"><![CDATA[IAsyncEnumerable<Option<T>>]]></see> and therefore returns an <see cref="IAsyncEnumerable{T}"/>.
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

    /// <summary>
    /// Projects and filters an <see cref="IAsyncEnumerable{T}"/> at the same time.
    /// This is done by filtering out any empty <see cref="Option{T}"/> values returned by the <paramref name="selector"/>.
    /// The index of each source element is used in the projected form of that element.
    /// </summary>
    [Pure]
    public static IAsyncEnumerable<TResult> WhereSelect<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, int, Option<TResult>> selector)
        where TResult : notnull
        => source.Select(selector).SelectMany(ToAsyncEnumerable);

    /// <inheritdoc cref="WhereSelect{TSource,TResult}(IAsyncEnumerable{TSource},Func{TSource,Option{TResult}})"/>
    [Pure]
    public static IAsyncEnumerable<TResult> WhereSelectAwait<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<Option<TResult>>> selector)
        where TResult : notnull
        => source.Select((TSource item, CancellationToken _) => selector(item)).SelectMany(ToAsyncEnumerable);

    /// <inheritdoc cref="WhereSelect{TSource,TResult}(IAsyncEnumerable{TSource},Func{TSource,int,Option{TResult}})"/>
    [Pure]
    public static IAsyncEnumerable<TResult> WhereSelectAwait<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, int, ValueTask<Option<TResult>>> selector)
        where TResult : notnull
        => source.Select((item, index, _) => selector(item, index)).SelectMany(ToAsyncEnumerable);

    /// <inheritdoc cref="WhereSelect{TSource,TResult}(IAsyncEnumerable{TSource},Func{TSource,Option{TResult}})"/>
    [Pure]
    public static IAsyncEnumerable<TResult> WhereSelectAwaitWithCancellation<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<Option<TResult>>> selector)
        where TResult : notnull
        => source.Select(selector).SelectMany(ToAsyncEnumerable);

    /// <inheritdoc cref="WhereSelect{TSource,TResult}(IAsyncEnumerable{TSource},Func{TSource,int,Option{TResult}})"/>
    [Pure]
    public static IAsyncEnumerable<TResult> WhereSelectAwaitWithCancellation<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, ValueTask<Option<TResult>>> selector)
        where TResult : notnull
        => source.Select(selector).SelectMany(ToAsyncEnumerable);

    private static IAsyncEnumerable<TItem> ToAsyncEnumerable<TItem>(Option<TItem> option)
        where TItem : notnull
        => option.ToAsyncEnumerable();
}
#endif
