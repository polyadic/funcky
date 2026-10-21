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
