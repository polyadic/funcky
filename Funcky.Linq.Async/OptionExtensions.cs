using System.Collections.Generic;
using System.Linq;
using Funcky.Monads;

namespace Funcky.Linq.Async
{
    internal static class OptionExtensions
    {
        public static IAsyncEnumerable<TItem> ToAsyncEnumerable<TItem>(this Option<TItem> option)
            where TItem : notnull
            => option.ToEnumerable().ToAsyncEnumerable();
    }
}
