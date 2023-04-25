using System.Diagnostics;
using static System.Diagnostics.DebuggerBrowsableState;

namespace Funcky.Monads;

public readonly partial struct Either<TLeft, TRight>
    where TLeft : notnull
    where TRight : notnull
{
    [Pure]
    [DebuggerBrowsable(Never)]
    public int Count
        => 2;

    [Pure]
    [DebuggerBrowsable(Never)]
    public object? this[int index]
        => _side switch
        {
            Side.Right => ChoseByIndex(null, _right, index),
            Side.Left => ChoseByIndex(_left, null, index),
            Side.Uninitialized => throw new NotSupportedException("Either constructed via default instead of a factory function"),
            _ => throw new IndexOutOfRangeException("Index was out of range."),
        };

    private static object? ChoseByIndex(object? left, object? right, int index)
        => index switch
        {
            0 => left,
            1 => right,
            _ => throw new IndexOutOfRangeException("Index was out of range."),
        };
}
