using System.ComponentModel;
using System.Diagnostics;

namespace Funcky.Analyzers;

internal readonly partial struct Option<TItem>
{
    private const int NoneLength = 0;
    private const int SomeLength = 1;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Count
        => _hasItem
            ? SomeLength
            : NoneLength;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public TItem this[int index]
        => _hasItem && index is 0
            ? _item
            : throw new IndexOutOfRangeException("Index was out of range.");
}
