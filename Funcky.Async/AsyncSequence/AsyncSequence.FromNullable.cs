namespace Funcky;

public static partial class AsyncSequence
{
    /// <returns>An <see cref="IEnumerable{T}" /> consisting of a single item or zero items.</returns>
    [Pure]
    public static IAsyncEnumerable<T> FromNullable<T>(T? item)
        where T : class
        => item is null
            ? AsyncEnumerable.Empty<T>()
            : Return(item);

    /// <inheritdoc cref="FromNullable{T}(T)"/>
    [Pure]
    public static IAsyncEnumerable<T> FromNullable<T>(T? item)
        where T : struct
        => item.HasValue
            ? Return(item.Value)
            : AsyncEnumerable.Empty<T>();
}
