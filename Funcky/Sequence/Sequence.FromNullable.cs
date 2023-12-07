#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
namespace Funcky;

public static partial class Sequence
{
    /// <returns>An <see cref="IEnumerable{T}" /> consisting of a single item or zero items.</returns>
    [Pure]
    public static IEnumerable<TResult> FromNullable<TResult>(TResult? element)
        where TResult : class
        => element is null ? [] : Return(element);

    /// <inheritdoc cref="FromNullable{T}(T)"/>
    [Pure]
    public static IEnumerable<TResult> FromNullable<TResult>(TResult? element)
        where TResult : struct
        => element.HasValue ? Return(element.Value) : [];
}
