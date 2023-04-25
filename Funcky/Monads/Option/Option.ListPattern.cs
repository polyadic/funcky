using System.Diagnostics;
using static System.Diagnostics.DebuggerBrowsableState;

namespace Funcky.Monads;

public readonly partial struct Option<TItem>
    where TItem : notnull
{
    private const int NoneLength = 0;
    private const int SomeLength = 1;

    [Pure]
    [DebuggerBrowsable(Never)]
    public int Count
        => _hasItem
            ? SomeLength
            : NoneLength;

    [Pure]
    [DebuggerBrowsable(Never)]
    public TItem this[int index]
        => _hasItem && index is 0
            ? _item
            : throw new IndexOutOfRangeException("Index was out of range.");
}
