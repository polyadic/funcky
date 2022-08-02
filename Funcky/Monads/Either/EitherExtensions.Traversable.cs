using System.Diagnostics.CodeAnalysis;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Funcky.Monads;

public static partial class EitherExtensions
{
    [Pure]
    public static Option<Either<TLeft, TItem>> Traverse<TLeft, TRight, TItem>(
        this Either<TLeft, TRight> either,
        Func<TRight, Option<TItem>> selector)
        where TItem : notnull
        => either.Select(selector).Sequence();

    [Pure]
    public static Option<Either<TLeft, TRight>> Sequence<TLeft, TRight>(
        this Either<TLeft, Option<TRight>> either)
        where TRight : notnull
        => either.Match(
            left: static left => Option.Return(Either<TLeft, TRight>.Left(left)),
            right: static right => right.Select(Either<TLeft>.Return));

    [Pure]
    public static Result<Either<TLeft, TValidResult>> Traverse<TLeft, TRight, TValidResult>(
        this Either<TLeft, TRight> either,
        Func<TRight, Result<TValidResult>> selector)
        => either.Select(selector).Sequence();

    [Pure]
    public static Result<Either<TLeft, TRight>> Sequence<TLeft, TRight>(
        this Either<TLeft, Result<TRight>> either)
        => either.Match(
            left: static left => Result.Return(Either<TLeft, TRight>.Left(left)),
            right: static right => right.Select(Either<TLeft>.Return));

    [Pure]
    public static Lazy<Either<TLeft, T>> Traverse<TLeft, TRight, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>(
        this Either<TLeft, TRight> either,
        Func<TRight, Lazy<T>> selector)
        => either.Select(selector).Sequence();

    [Pure]
    public static Lazy<Either<TLeft, TRight>> Sequence<TLeft, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TRight>(
        this Either<TLeft, Lazy<TRight>> either)
        => either.Match(
            left: static left => Lazy.Return(Either<TLeft, TRight>.Left(left)),
            right: SequenceLazy<TLeft, TRight>.Right);

    [Pure]
    public static IEnumerable<Either<TLeft, T>> Traverse<TLeft, TRight, T>(
        this Either<TLeft, TRight> either,
        Func<TRight, IEnumerable<T>> selector)
        => either.Select(selector).Sequence();

    [Pure]
    public static IEnumerable<Either<TLeft, TRight>> Sequence<TLeft, TRight>(
        this Either<TLeft, IEnumerable<TRight>> either)
        => either.Match(
            left: static left => Funcky.Sequence.Return(Either<TLeft, TRight>.Left(left)),
            right: static right => right.Select(Either<TLeft>.Return));

    [Pure]
    public static Reader<TEnvironment, Either<TLeft, TResult>> Traverse<TLeft, TRight, TEnvironment, TResult>(
        this Either<TLeft, TRight> either,
        Func<TRight, Reader<TEnvironment, TResult>> selector)
        => either.Select(selector).Sequence();

    [Pure]
    public static Reader<TEnvironment, Either<TLeft, TRight>> Sequence<TLeft, TEnvironment, TRight>(
        this Either<TLeft, Reader<TEnvironment, TRight>> either)
        => either.Match(
            left: static left => Reader<TEnvironment>.Return(Either<TLeft, TRight>.Left(left)),
            right: static right => right.Select(Either<TLeft>.Return));

    // Workaround for https://github.com/dotnet/linker/issues/1416
    private static class SequenceLazy<TLeft, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TRight>
    {
        private static Func<Lazy<TRight>, Lazy<Either<TLeft, TRight>>>? _right;

        public static Func<Lazy<TRight>, Lazy<Either<TLeft, TRight>>> Right => _right ??= (right => right.Select(Either<TLeft>.Return<TRight>));
    }
}
