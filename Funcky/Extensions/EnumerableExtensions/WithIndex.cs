using Funcky.Internal;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Returns a sequence mapping each element together with an Index starting at 0. The returned struct is deconstructible.
        /// </summary>
        /// <param name="source">The source sequence.</param>
        /// <typeparam name="TSource">Type of the elements in <paramref name="source"/> sequence.</typeparam>
        /// <returns>Returns a sequence mapping each element together with an Index starting at 0.</returns>
        [Pure]
        public static IEnumerable<ValueWithIndex<TSource>> WithIndex<TSource>(this IEnumerable<TSource> source)
            => source switch
            {
                IList<TSource> list => ListWithSelector.Create(list, ValueWithIndex),
                _ => source.Select(ValueWithIndex<TSource>),
            };

        private static Func<TSource, int, ValueWithIndex<TSource>> ValueWithIndex<TSource>(IList<TSource> dummy)
            => ValueWithIndex<TSource>;

        private static ValueWithIndex<TValue> ValueWithIndex<TValue>(TValue value, int index)
            => new(value, index);
    }
}
