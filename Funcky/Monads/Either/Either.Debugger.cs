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
