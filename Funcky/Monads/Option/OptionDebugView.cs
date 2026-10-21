using System.Diagnostics;
using static System.Diagnostics.DebuggerBrowsableState;

namespace Funcky.Monads;

internal sealed class OptionDebugView<T>(Option<T> option)
    where T : notnull
{
    [DebuggerBrowsable(RootHidden)]
    public object Value => option.Match(
        none: () => (object)new { },
        some: value => new { Value = value });
}
