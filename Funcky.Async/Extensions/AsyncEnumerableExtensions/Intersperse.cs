using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Async.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>Returns a sequence with the items of the source sequence interspersed with the given <paramref name="element"/>.</summary>
        [Pure]
        public static IAsyncEnumerable<TSource> Intersperse<TSource>(this IAsyncEnumerable<TSource> source, TSource element)
            => source.WithFirst().SelectMany(item => item.IsFirst
                ? AsyncSequence.Return(item.Value)
                : AsyncSequence.Return(element).Append(item.Value));
    }
}
