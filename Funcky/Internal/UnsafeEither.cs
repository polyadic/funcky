using System.Diagnostics.CodeAnalysis;

namespace Funcky.Internal;

internal readonly struct UnsafeEither<TLeft, TRight>
{
    public readonly TLeft? LeftValue;
    public readonly TRight? RightValue;

    private UnsafeEither(bool isRight, TLeft? leftValue = default, TRight? rightValue = default)
    {
        IsRight = isRight;
        LeftValue = leftValue;
        RightValue = rightValue;
    }

    [MemberNotNullWhen(false, nameof(LeftValue))]
    [MemberNotNullWhen(true, nameof(RightValue))]
    public bool IsRight { get; }

    public static UnsafeEither<TLeft, TRight> Left(TLeft left) => new(isRight: false, leftValue: left);

    public static UnsafeEither<TLeft, TRight> Right(TRight right) => new(isRight: true, rightValue: right);
}

internal static class UnsafeEither
{
    extension<TItem>(UnsafeEither<Unit, TItem> either)
        where TItem : notnull
    {
        public Option<TItem> ToOption()
            => either.IsRight
                ? either.RightValue
                : Option<TItem>.None;
    }

    extension<TLeft, TRight>(UnsafeEither<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
    {
        public Either<TLeft, TRight> ToEither()
            => either.IsRight
                ? Either<TLeft, TRight>.Right(either.RightValue)
                : Either<TLeft, TRight>.Left(either.LeftValue);
    }

    extension<TValidResult>(UnsafeEither<Exception, TValidResult> either)
        where TValidResult : notnull
    {
        public Result<TValidResult> ToResult()
            => either.IsRight
                ? Result.Ok(either.RightValue)
                : Result<TValidResult>.Error(either.LeftValue);
    }

    public static UnsafeEither<Unit, TItem> FromOption<TItem>(Option<TItem> option)
        where TItem : notnull
        => option.Match(
            none: static () => UnsafeEither<Unit, TItem>.Left(Unit.Value),
            some: UnsafeEither<Unit, TItem>.Right);

    public static UnsafeEither<TLeft, TRight> FromEither<TLeft, TRight>(Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
        => either.Match(
            left: UnsafeEither<TLeft, TRight>.Left,
            right: UnsafeEither<TLeft, TRight>.Right);

    public static UnsafeEither<Exception, TValidResult> FromResult<TValidResult>(Result<TValidResult> result)
        where TValidResult : notnull
        => result.Match(
            error: UnsafeEither<Exception, TValidResult>.Left,
            ok: UnsafeEither<Exception, TValidResult>.Right);
}
