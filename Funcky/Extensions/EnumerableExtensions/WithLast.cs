using Funcky.Internal;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <param name="source">The source sequence.</param>
    /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// Returns a sequence mapping each element into a type which has an IsLast property which is true for the last element of the sequence. The returned struct is deconstructible.
        /// </summary>
        /// <returns>Returns a sequence mapping each element into a type which has an IsLast property which is true for the last element of the sequence.</returns>
        [Pure]
        public IEnumerable<ValueWithLast<TSource>> WithLast()
            => ArgumentValidator.NotNull(source) switch
            {
                IList<TSource> list => ListWithSelector.Create(list, ValueWithLast),
                _ => source.WithLastImplementation(),
            };

        private IEnumerable<ValueWithLast<TSource>> WithLastImplementation()
        {
            using var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                yield break;
            }

            var current = enumerator.Current;
            while (enumerator.MoveNext())
            {
                yield return new ValueWithLast<TSource>(current, false);
                current = enumerator.Current;
            }

            yield return new ValueWithLast<TSource>(current, true);
        }
    }

    private static Func<TSource, int, ValueWithLast<TSource>> ValueWithLast<TSource>(IList<TSource> list)
        => (value, index)
            => new(value, index == list.Count - 1);
}
