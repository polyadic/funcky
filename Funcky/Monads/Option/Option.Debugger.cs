using System.Diagnostics;

namespace Funcky.Monads
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [DebuggerTypeProxy(typeof(OptionDebugView<>))]
    public readonly partial struct Option<TItem>
    {
        private string DebuggerDisplay => Match(
            none: "None",
            some: _ => "Some");
    }

    internal sealed class OptionDebugView<T>
        where T : notnull
    {
        private readonly Option<T> _option;

        public OptionDebugView(Option<T> option) => _option = option;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object Value => _option.Match(
            none: () => (object)new { },
            some: value => new { Value = value });
    }
}
