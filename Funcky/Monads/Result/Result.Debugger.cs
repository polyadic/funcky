using System.Diagnostics;
using static System.Diagnostics.DebuggerBrowsableState;

namespace Funcky.Monads;

[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
[DebuggerTypeProxy(typeof(ResultDebugView<>))]
public readonly partial struct Result<TValidResult>
{
    [DebuggerBrowsable(Never)]
    private string DebuggerDisplay => Match(
        uninitialized: static () => "Uninitialized",
        ok: static _ => "Ok",
        error: static _ => "Error");
}
