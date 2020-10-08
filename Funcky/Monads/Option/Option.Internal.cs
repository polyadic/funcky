namespace Funcky.Monads
{
    public readonly partial struct Option<TItem>
    {
        internal bool TryGetValue(
            #if NULLABLE_ATTRIBUTES_SUPPORTED
            [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)]
            #endif
            out TItem item)
        {
            item = _hasItem ? _item : default!;
            return _hasItem;
        }
    }
}
