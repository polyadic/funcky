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
        /// Returns a sequence mapping each element into a type which has an IsFirst property which is true for the first element of the sequence. The returned struct is deconstructible.
        /// </summary>
        /// <returns>Returns a sequence mapping each element into a type which has an IsFirst property which is true for the first element of the sequence.</returns>
        [Pure]
        public IEnumerable<ValueWithFirst<TSource>> WithFirst()
            => ArgumentValidator.NotNull(source) switch
            {
                IList<TSource> list => ListWithSelector.Create(list, ValueWithFirst),
                _ => source.WithFirstImplementation(),
            };

        private IEnumerable<ValueWithFirst<TSource>> WithFirstImplementation()
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

    private static Func<TSource, int, ValueWithFirst<TSource>> ValueWithFirst<TSource>(IList<TSource> list)
        => (value, index)
            => new(value, index == 0);
}
