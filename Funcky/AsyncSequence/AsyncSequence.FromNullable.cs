namespace Funcky;

public static partial class AsyncSequence
{
    /// <returns>An <see cref="IEnumerable{T}" /> consisting of a single item or zero items.</returns>
    [Pure]
    public static IAsyncEnumerable<TResult> FromNullable<TResult>(TResult? element)
        where TResult : class
        => element is null
            ? AsyncEnumerable.Empty<TResult>()
            : Return(element);

    /// <inheritdoc cref="FromNullable{T}(T)"/>
    [Pure]
    public static IAsyncEnumerable<TResult> FromNullable<TResult>(TResult? element)
        where TResult : struct
        => element.HasValue
            ? Return(element.Value)
            : AsyncEnumerable.Empty<TResult>();
}
