using System.Collections.Immutable;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <param name="source">The source sequence.</param>
    /// <typeparam name="TSource">The type of the elements in the enumerable.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// The PowerSet function returns a sequence with the set of all subsets.
        /// </summary>
        /// <returns>Returns an sequence which includes all subsets of the given sequence.</returns>
        /// <remarks>The PowerSet function returns a sequence with 2^n elements where n is the number of elements in the source sequence.
        /// This means it is only viable for small source sequences. The whole source is enumerated and held in memory before the
        /// first subset is produced, and the implementation recurses once per source element.</remarks>
        public IEnumerable<IEnumerable<TSource>> PowerSet()
            => PowerSetIterator(ArgumentValidator.NotNull(source));
    }

    extension<TSource>(IEnumerator<TSource> source)
    {
        private IEnumerable<ImmutableStack<TSource>> PowerSetEnumerator()
        {
            if (source.MoveNext())
            {
                var temp = source.Current;
                foreach (var set in source.PowerSetEnumerator())
                {
                    yield return set;
                    yield return set.Push(temp);
                }
            }
            else
            {
                yield return ImmutableStack<TSource>.Empty;
            }
        }
    }

    private static IEnumerable<IEnumerable<TSource>> PowerSetIterator<TSource>(IEnumerable<TSource> source)
    {
        using var sourceEnumerator = source.GetEnumerator();

        foreach (var set in PowerSetEnumerator(sourceEnumerator))
        {
            yield return set;
        }
    }
}
