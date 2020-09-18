using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        public static IEnumerable<T> Interleave<T>(this IEnumerable<T> sequence, params IEnumerable<T>[] otherSequences)
            => Interleave(new[] { sequence }.Concat(otherSequences));

        [Pure]
        public static IEnumerable<TSource> Interleave<TSource>(this IEnumerable<IEnumerable<TSource>> source)
        {
            List<IEnumerator<TSource>> sourceEnumerators = source.Select(s => s.GetEnumerator()).ToList();

            List<IEnumerator<TSource>> enumerators = sourceEnumerators;
            while (enumerators.Count > 0)
            {
                enumerators = enumerators.Where(e => e.MoveNext()).ToList();
                foreach (var enumerator in enumerators)
                {
                    yield return enumerator.Current!;
                }
            }

            sourceEnumerators.ForEach(enumerator => enumerator.Dispose());
        }
    }
}
