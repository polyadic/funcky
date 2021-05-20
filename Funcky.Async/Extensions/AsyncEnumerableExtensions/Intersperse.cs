using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Funcky.Internal;

namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>Returns a sequence with the items of the source sequence interspersed with the given <paramref name="element"/>.</summary>
        [Pure]
        public static IAsyncEnumerable<TSource> Intersperse<TSource>(this IAsyncEnumerable<TSource> source, TSource element)
            => source.SelectMany((item, index) => IsFirst(index)
                ? AsyncEnumerable.Repeat(item, 1)
                : ImmutableArray.Create(element, item).ToAsyncEnumerable());

        private static bool IsFirst(int index) => index == 0;
    }
}
