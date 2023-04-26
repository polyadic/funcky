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

internal sealed class ResultDebugView<TValidResult>
    where TValidResult : notnull
{
    private readonly Result<TValidResult> _option;

    public ResultDebugView(Result<TValidResult> option) => _option = option;

    [DebuggerBrowsable(RootHidden)]
    public object Value => _option.Match(
        ok: value => (object)new { Value = value },
        error: exception => new { Exception = exception });
}
