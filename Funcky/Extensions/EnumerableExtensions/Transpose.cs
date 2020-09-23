using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<IEnumerable<TSource>> Transpose<TSource>(this IEnumerable<IEnumerable<TSource>> source)
        {
            var rows = source.Count();

            return rows == 0
                ? Enumerable.Empty<IEnumerable<TSource>>()
                : source.Interleave().Chunk(rows);
        }
    }
}
