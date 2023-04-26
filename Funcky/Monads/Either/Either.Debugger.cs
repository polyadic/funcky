using System.Diagnostics;
using static System.Diagnostics.DebuggerBrowsableState;

namespace Funcky.Monads;

[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
[DebuggerTypeProxy(typeof(EitherDebugView<,>))]
public readonly partial struct Either<TLeft, TRight>
{
    [DebuggerBrowsable(Never)]
    private string DebuggerDisplay => Match(
        uninitialized: static () => "default",
        left: static _ => "Left",
        right: static _ => "Right");
}

internal sealed class EitherDebugView<TLeft, TRight>
    where TLeft : notnull
    where TRight : notnull
{
    private readonly Either<TLeft, TRight> _either;

    public EitherDebugView(Either<TLeft, TRight> either) => _either = either;

    [DebuggerBrowsable(RootHidden)]
    public object Value => _either.Match<object>(
        uninitialized: static () => new { },
        left: left => new { Value = left },
        right: right => new { Value = right });
}
