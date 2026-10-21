using System.Diagnostics;
using static System.Diagnostics.DebuggerBrowsableState;

namespace Funcky.Monads;

internal sealed class ResultDebugView<TValidResult>(Result<TValidResult> result)
    where TValidResult : notnull
{
    [DebuggerBrowsable(RootHidden)]
    public object Value => result.Match(
        uninitialized: static () => new { },
        ok: static value => (object)new { Value = value },
        error: static exception => new { Exception = exception });
}
