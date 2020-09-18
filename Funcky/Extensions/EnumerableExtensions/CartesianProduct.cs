using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<(TFirstSource First, TSecondSource Second)> CartesianProduct<TFirstSource,
            TSecondSource>(
            this IEnumerable<TFirstSource> firstSequence, IEnumerable<TSecondSource> secondSequence) =>
                CartesianProduct(firstSequence, secondSequence, ValueTuple.Create);

        [Pure]
        public static IEnumerable<TResult> CartesianProduct<TSource1, TSource2, TResult>(
            this IEnumerable<TSource1> firstSequence,
            IEnumerable<TSource2> secondSequence,
            Func<TSource1, TSource2, TResult> resultSelector) =>
                from first in firstSequence
                from second in secondSequence
                select resultSelector(first, second);
    }
}
