using static Funcky.Async.ValueTaskFactory;

namespace Funcky.Monads;

public static class EitherAsyncExtensions
{
    [Pure]
    public static IAsyncEnumerable<Either<TLeft, T>> Traverse<TLeft, TRight, T>(
        this Either<TLeft, TRight> either,
        Func<TRight, IAsyncEnumerable<T>> selector)
        => either.Select(selector).Sequence();

    [Pure]
    public static IAsyncEnumerable<Either<TLeft, TRight>> Sequence<TLeft, TRight>(
        this Either<TLeft, IAsyncEnumerable<TRight>> either)
        => either.Match(
            left: static left => AsyncSequence.Return(Either<TLeft, TRight>.Left(left)),
            right: static right => right.Select(Either<TLeft>.Return));

    [Pure]
    public static Task<Either<TLeft, T>> Traverse<TLeft, TRight, T>(
        this Either<TLeft, TRight> either,
        Func<TRight, Task<T>> selector)
        => either.Select(selector).Sequence();

    [Pure]
    public static Task<Either<TLeft, TRight>> Sequence<TLeft, TRight>(
        this Either<TLeft, Task<TRight>> either)
        => either.Match(
            left: static left => Task.FromResult(Either<TLeft, TRight>.Left(left)),
            right: static async right => Either<TLeft>.Return(await right.ConfigureAwait(false)));

    [Pure]
    public static ValueTask<Either<TLeft, T>> Traverse<TLeft, TRight, T>(
        this Either<TLeft, TRight> either,
        Func<TRight, ValueTask<T>> selector)
        => either.Select(selector).Sequence();

    [Pure]
    public static ValueTask<Either<TLeft, TRight>> Sequence<TLeft, TRight>(
        this Either<TLeft, ValueTask<TRight>> either)
        => either.Match<ValueTask<Either<TLeft, TRight>>>(
            left: static left => ValueTaskFromResult(Either<TLeft, TRight>.Left(left)),
            right: static async right => Either<TLeft>.Return(await right.ConfigureAwait(false)));
}
