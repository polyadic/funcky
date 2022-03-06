using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Funcky.Monads
{
    public readonly partial struct Option<TItem>
    {
        /// <summary>Extracts the value out of this option. This function should be used as a last resort.</summary>
        /// <remarks>The only allowed uses of this function are as part of a loop condition in an iterator or as part of a catch filter clause (<c>catch ... when</c>).
        /// All other uses will result in a <c>λ0001: Disallowed use of TryGetValue</c> error.</remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool TryGetValue(
            [MaybeNullWhen(false)]
            out TItem item)
        {
            item = _hasItem ? _item : default!;
            return _hasItem;
        }
    }
}
