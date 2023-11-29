using System.Diagnostics;
using static System.Diagnostics.DebuggerBrowsableState;

namespace Funcky.Monads;

[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
[DebuggerTypeProxy(typeof(OptionDebugView<>))]
public readonly partial struct Option<TItem>
{
    [DebuggerBrowsable(Never)]
    private string DebuggerDisplay => Match(
        none: "None",
        some: _ => "Some");
}

internal sealed class OptionDebugView<T>(Option<T> option)
    where T : notnull
{
    [DebuggerBrowsable(RootHidden)]
    public object Value => option.Match(
        none: () => (object)new { },
        some: value => new { Value = value });
}
