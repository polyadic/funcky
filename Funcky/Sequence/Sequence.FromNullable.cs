namespace Funcky;

public static partial class Sequence
{
    /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).FromNullable{TResult}(TResult?)"/>
    [Pure]
    public static IEnumerable<TResult> FromNullable<TResult>(TResult? element)
        where TResult : class
        => Enumerable.FromNullable(element);

    /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).FromNullable{TResult}(TResult?)"/>
    [Pure]
    public static IEnumerable<TResult> FromNullable<TResult>(TResult? element)
        where TResult : struct
        => Enumerable.FromNullable(element);
}
