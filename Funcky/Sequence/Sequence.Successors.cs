namespace Funcky;

public static partial class Sequence
{
    /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).Successors{TResult}(Option{TResult}, Func{TResult, Option{TResult}})"/>
    [Pure]
    public static IEnumerable<TResult> Successors<TResult>(Option<TResult> first, Func<TResult, Option<TResult>> successor)
        where TResult : notnull
        => Enumerable.Successors(first, successor);

    /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).Successors{TResult}(TResult, Func{TResult, Option{TResult}})"/>
    [Pure]
    public static IEnumerable<TResult> Successors<TResult>(TResult first, Func<TResult, Option<TResult>> successor)
        where TResult : notnull
        => Enumerable.Successors(first, successor);

    /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).Successors{TResult}(Option{TResult}, Func{TResult, TResult})"/>
    [Pure]
    public static IEnumerable<TResult> Successors<TResult>(Option<TResult> first, Func<TResult, TResult> successor)
        where TResult : notnull
        => Enumerable.Successors(first, successor);

    /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).Successors{TResult}(TResult, Func{TResult, TResult})"/>
    [Pure]
    public static IEnumerable<TResult> Successors<TResult>(TResult first, Func<TResult, TResult> successor)
        => Enumerable.Successors(first, successor);
}
