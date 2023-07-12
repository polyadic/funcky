using System.Diagnostics.CodeAnalysis;
using Funcky.CodeAnalysis;

namespace Funcky.Monads;

/// <remarks>
/// Either values constructed using <c>default</c> are in an invalid state.
/// Any attempt to perform actions on such a value will throw a <see cref="NotSupportedException"/>.
/// </remarks>
public readonly partial struct Either<TLeft, TRight> : IEquatable<Either<TLeft, TRight>>
    where TLeft : notnull
    where TRight : notnull
{
    private readonly TLeft _left;
    private readonly TRight _right;
    private readonly Side _side;

    private Either(TLeft left)
    {
        if (left is null)
        {
            throw new ArgumentNullException(nameof(left));
        }

        _left = left;
        _right = default!;
        _side = Side.Left;
    }

    private Either(TRight right)
    {
        if (right is null)
        {
            throw new ArgumentNullException(nameof(right));
        }

        _left = default!;
        _right = right;
        _side = Side.Right;
    }

    private enum Side : byte
    {
        Uninitialized,
        Left,
        Right,
    }

    [Pure]
    public static bool operator ==(Either<TLeft, TRight> left, Either<TLeft, TRight> right) => left.Equals(right);

    [Pure]
    public static bool operator !=(Either<TLeft, TRight> left, Either<TLeft, TRight> right) => !left.Equals(right);

    [Pure]
    public static Either<TLeft, TRight> Left(TLeft left) => new(left);

    [Pure]
    public static Either<TLeft, TRight> Right(TRight right) => new(right);

    [Pure]
    [UseWithArgumentNames]
    public TMatchResult Match<TMatchResult>(Func<TLeft, TMatchResult> left, Func<TRight, TMatchResult> right)
        => TryGetValue(out var rightValue, out var leftValue)
            ? right(rightValue)
            : left(leftValue);

    [UseWithArgumentNames]
    public void Switch(Action<TLeft> left, Action<TRight> right)
    {
        if (TryGetValue(out var rightValue, out var leftValue))
        {
            right(rightValue);
        }
        else
        {
            left(leftValue);
        }
    }

    [Pure]
    public override bool Equals(object? obj)
        => obj is Either<TLeft, TRight> other && Equals(other);

    [Pure]
    public bool Equals(Either<TLeft, TRight> other)
        => Equals(_side, other._side)
            && Equals(_right, other._right)
            && Equals(_left, other._left);

    [Pure]
    public override int GetHashCode()
        => Match(
            left: left => left?.GetHashCode(),
            right: right => right?.GetHashCode()) ?? 0;

    [Pure]
    public Either<TRight, TLeft> Flip()
        => Match(
            left: Either<TRight, TLeft>.Right,
            right: Either<TRight, TLeft>.Left);

    [Pure]
    public override string ToString()
        => Match(
            left: static left => $"Left({left})",
            right: static right => $"Right({right})");

    [Pure]
    [UseWithArgumentNames]
    internal TMatchResult Match<TMatchResult>(Func<TMatchResult> uninitialized, Func<TLeft, TMatchResult> left, Func<TRight, TMatchResult> right)
        => _side switch
        {
            Side.Right => right(_right),
            Side.Left => left(_left),
            Side.Uninitialized => uninitialized(),
            _ => throw new NotSupportedException($"Unreachable: Enum variant {_side} is not handled"),
        };

    private bool TryGetValue([NotNullWhen(true)] out TRight? right, [NotNullWhen(false)] out TLeft? left)
    {
        right = _right;
        left = _left;
        return _side switch
        {
            Side.Right => true,
            Side.Left => false,
            Side.Uninitialized => throw new NotSupportedException($"Either constructed via default instead of a factory function ({nameof(Either<TLeft, TRight>)}.{nameof(Left)} or {nameof(Either<TLeft, TRight>)}.{nameof(Right)})"),
            _ => throw new NotSupportedException($"Unreachable: Enum variant {_side} is not handled"),
        };
    }
}

public static class Either<TLeft>
    where TLeft : notnull
{
    [Pure]
    public static Either<TLeft, TRight> Return<TRight>(TRight right)
        where TRight : notnull
        => Either<TLeft, TRight>.Right(right);
}
