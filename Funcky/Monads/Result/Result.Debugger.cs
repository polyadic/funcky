using System.Diagnostics;
using static System.Diagnostics.DebuggerBrowsableState;

namespace Funcky.Monads;

[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
[DebuggerTypeProxy(typeof(ResultDebugView<>))]
public readonly partial struct Result<TValidResult>
{
    [DebuggerBrowsable(Never)]
    private string DebuggerDisplay => Match(
        ok: _ => "Ok",
        error: _ => "Error");
}

internal sealed class ResultDebugView<TValidResult>(Result<TValidResult> result)
    where TValidResult : notnull
{
    [DebuggerBrowsable(RootHidden)]
    public object Value => result.Match(
        ok: value => (object)new { Value = value },
        error: exception => new { Exception = exception });
}
