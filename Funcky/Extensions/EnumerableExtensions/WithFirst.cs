using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns a sequence mapping each element into a type which has an IsFirst property which is true for the first element of the sequence. The returned struct is deconstructible.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <returns>Returns a sequence mapping each element into a type which has an IsFirst property which is true for the first element of the sequence.</returns>
        [Pure]
        public static IEnumerable<ValueWithFirst<TSource>> WithFirst<TSource>(this IEnumerable<TSource> source)
            => source switch
            {
                IList<TSource> list => new ListWithSelector<TSource, ValueWithFirst<TSource>>(list, ValueWithFirst),
                _ => source.WithFirstImplementation(),
            };

        private static Func<TSource, int, ValueWithFirst<TSource>> ValueWithFirst<TSource>(IList<TSource> list)
            => (value, index)
                => new(value, index == 0);

        private static IEnumerable<ValueWithFirst<TSource>> WithFirstImplementation<TSource>(this IEnumerable<TSource> source)
        {
            using var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                yield break;
            }

            yield return new ValueWithFirst<TSource>(enumerator.Current, true);

            while (enumerator.MoveNext())
            {
                yield return new ValueWithFirst<TSource>(enumerator.Current, false);
            }
        }
    }
}
