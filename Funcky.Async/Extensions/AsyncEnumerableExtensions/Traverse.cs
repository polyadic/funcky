using System.Diagnostics.CodeAnalysis;

namespace Funcky.Async.Extensions;

public static partial class AsyncEnumerableExtensions
{
    [SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters", Justification = "<Pending>")]
    public static ValueTask<Either<TLeft, IReadOnlyList<TRight>>> TraverseAsync<TSource, TLeft, TRight>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, Either<TLeft, TRight>> selector,
        CancellationToken cancellationToken = default)
        => source.Select(selector).SequenceAsync(cancellationToken);

    [SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters", Justification = "<Pending>")]
    public static ValueTask<Option<IReadOnlyList<TItem>>> TraverseAsync<TSource, TItem>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, Option<TItem>> selector,
        CancellationToken cancellationToken = default)
        where TItem : notnull
        => source.Select(selector).SequenceAsync(cancellationToken);

    [SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters", Justification = "<Pending>")]
    public static ValueTask<Result<IReadOnlyList<TValidResult>>> TraverseAsync<TSource, TValidResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, Result<TValidResult>> selector,
        CancellationToken cancellationToken = default)
        => source.Select(selector).SequenceAsync(cancellationToken);

    [Pure]
    public static Reader<TEnvironment, IAsyncEnumerable<TResult>> Traverse<TSource, TEnvironment, TResult>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, Reader<TEnvironment, TResult>> selector)
        => source.Select(selector).Sequence();

    [Pure]
    public static Lazy<IAsyncEnumerable<T>> Traverse<TSource, T>(
        IAsyncEnumerable<TSource> source,
        Func<TSource, Lazy<T>> selector)
        => source.Select(selector).Sequence();
}
