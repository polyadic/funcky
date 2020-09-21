using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<TSource> Merge<TSource>(this IEnumerable<TSource> sequence, IComparer<TSource> comparer, params IEnumerable<TSource>[] otherSequences)
            => ImmutableList.Create(sequence).AddRange(otherSequences).Merge(comparer);

        [Pure]
        public static IEnumerable<TSource> Merge<TSource>(this IEnumerable<IEnumerable<TSource>> sources, IComparer<TSource> comparer)
        {
            var sourceEnumerators = ImmutableList.Create<IEnumerator<TSource>>().AddRange(sources.Select(s => s.GetEnumerator()));
            var enumerators = sourceEnumerators.RemoveAll(s => !s.MoveNext());

            while (!enumerators.IsEmpty)
            {
                var minimum = FindMinimum(enumerators, comparer);
                yield return minimum.Current;
                if (!minimum.MoveNext())
                {
                    enumerators = enumerators.Remove(minimum);
                }
            }

            sourceEnumerators.ForEach(s => s.Dispose());
        }

        private static IEnumerator<TSource> FindMinimum<TSource>(ImmutableList<IEnumerator<TSource>> enumerators, IComparer<TSource> comparer)
        {
            var minimum = enumerators.First();

            foreach (var enumerator in enumerators.Skip(1).Where(e => comparer.Compare(minimum.Current, e.Current) > 0))
            {
                minimum = enumerator;
            }

            return minimum;
        }
    }
}
