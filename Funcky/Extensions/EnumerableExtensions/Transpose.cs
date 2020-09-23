using System;
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
            return source.Any()
                ? source.Interleave().Chunk(source.Count())
                : Enumerable.Empty<IEnumerable<TSource>>();
        }
    }
}
