using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>Returns a sequence with the items of the source sequence interspersed with the given <paramref name="element"/>.</summary>
        [Pure]
        public static IEnumerable<TSource> Intersperse<TSource>(this IEnumerable<TSource> source, TSource element)
            => source.WithFirst()
                .SelectMany(s => s.IsFirst
                    ? Sequence.Return(s.Value)
                    : ImmutableArray.Create(element, s.Value));
    }
}
