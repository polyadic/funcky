using System.ComponentModel;
using System.Diagnostics;
using Funcky.CodeAnalysis;

namespace Funcky.Monads;

public readonly partial struct Option<TItem>
    where TItem : notnull
{
    private const int NoneLength = 0;
    private const int SomeLength = 1;

    [Pure]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SyntaxSupportOnly(SyntaxSupportOnlyAttribute.ListPattern)]
    public int Count
        => _hasItem
            ? SomeLength
            : NoneLength;

    [Pure]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SyntaxSupportOnly(SyntaxSupportOnlyAttribute.ListPattern)]
    public TItem this[int index]
        => _hasItem && index is 0
            ? _item
            : throw new IndexOutOfRangeException("Index was out of range.");
}
