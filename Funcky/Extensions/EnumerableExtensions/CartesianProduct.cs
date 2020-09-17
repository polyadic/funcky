using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<(TFirstSource A, TSecondSource B)> CartesianProduct<TFirstSource, TSecondSource>(
            this IEnumerable<TFirstSource> first, IEnumerable<TSecondSource> second)
            where TFirstSource : notnull
            where TSecondSource : notnull
        {
            return from a in first
                   from b in second
                   select (a, b);
        }
    }
}
