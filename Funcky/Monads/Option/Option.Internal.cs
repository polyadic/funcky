using System.Diagnostics.CodeAnalysis;

namespace Funcky.Monads
{
    public readonly partial struct Option<TItem>
    {
        internal bool TryGetValue(
            [MaybeNullWhen(false)]
            out TItem item)
        {
            item = _hasItem ? _item : default!;
            return _hasItem;
        }
    }
}
