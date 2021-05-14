using System;

namespace Funcky.Monads.Lazy
{
    public static class Lazy
    {
        public static Lazy<TItem> Return<TItem>(Func<TItem> valueFactory)
            => new(valueFactory);
    }
}
