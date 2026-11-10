using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Funcky.Spike.Unions;

/// <summary>
/// Shape 3 for <c>Either&lt;L, R&gt;</c>: today's layout (<c>L</c> + <c>R</c> + side byte) with <c>[Union]</c> and
/// <c>IUnion</c>. The cases are the payload types themselves (<c>TLeft</c> and <c>TRight</c>), so a match reads
/// <c>either is TLeft left</c> / <c>either is TRight right</c> and costs no wrapper allocation.
/// <para><c>default</c> reports a <c>null</c> Value: the compiler then warns (CS8655) on a switch that does not handle
/// null, which is exactly the [NonDefaultable] contract Funcky has today, surfaced at compile time instead of only at
/// runtime.</para>
/// <para>Open question probed in the tests: <c>Either&lt;int, int&gt;</c>, where both cases are the same type.</para>
/// </summary>
[Union]
public readonly struct CustomUnionEither<TLeft, TRight> : IUnion, IEquatable<CustomUnionEither<TLeft, TRight>>
    where TLeft : notnull
    where TRight : notnull
{
    private readonly TLeft _left;
    private readonly TRight _right;
    private readonly Side _side;

    public CustomUnionEither(TLeft left)
    {
        ArgumentNullException.ThrowIfNull(left);
        _left = left;
        _right = default!;
        _side = Side.Left;
    }

    public CustomUnionEither(TRight right)
    {
        ArgumentNullException.ThrowIfNull(right);
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

    public object? Value => _side switch
    {
        Side.Left => _left,
        Side.Right => _right,
        _ => null,
    };

    /// <summary>Non-boxing access pattern: the compiler uses these instead of <c>Value</c> for type patterns.</summary>
    public bool HasValue => _side != Side.Uninitialized;

    public bool TryGetValue([NotNullWhen(true)] out TLeft? left)
    {
        left = _side == Side.Left ? _left : default;
        return _side == Side.Left;
    }

    public bool TryGetValue([NotNullWhen(true)] out TRight? right)
    {
        right = _side == Side.Right ? _right : default;
        return _side == Side.Right;
    }

    public static CustomUnionEither<TLeft, TRight> Left(TLeft left) => new(left);

    public static CustomUnionEither<TLeft, TRight> Right(TRight right) => new(right);

    public TResult Match<TResult>(Func<TLeft, TResult> left, Func<TRight, TResult> right)
        => _side switch
        {
            Side.Left => left(_left),
            Side.Right => right(_right),
            _ => throw new NotSupportedException("Either constructed via default is not supported."),
        };

    public CustomUnionEither<TLeft, TResult> Select<TResult>(Func<TRight, TResult> selector)
        where TResult : notnull
        => _side switch
        {
            Side.Left => new CustomUnionEither<TLeft, TResult>(_left),
            Side.Right => new CustomUnionEither<TLeft, TResult>(selector(_right)),
            _ => throw new NotSupportedException("Either constructed via default is not supported."),
        };

    public bool Equals(CustomUnionEither<TLeft, TRight> other)
        => _side == other._side
            && EqualityComparer<TLeft>.Default.Equals(_left, other._left)
            && EqualityComparer<TRight>.Default.Equals(_right, other._right);

    public override bool Equals(object? obj) => obj is CustomUnionEither<TLeft, TRight> other && Equals(other);

    public override int GetHashCode() => _side switch
    {
        Side.Left => _left.GetHashCode(),
        Side.Right => _right.GetHashCode(),
        _ => 0,
    };

    public override string ToString() => _side switch
    {
        Side.Left => $"Left({_left})",
        Side.Right => $"Right({_right})",
        _ => "Uninitialized",
    };
}
