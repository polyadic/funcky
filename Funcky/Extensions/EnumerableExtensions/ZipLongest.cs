namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
        /// </summary>
        /// <param name="left">The left sequence to merge.</param>
        /// <typeparam name="TLeft">Type of the elements in <paramref name="left"/> sequence.</typeparam>
        /// <param name="right">The right sequence to merge.</param>
        /// <typeparam name="TRight">Type of the elements in <paramref name="right"/> sequence.</typeparam>
        /// <returns>A sequence that contains merged elements of two input sequences.</returns>
        [Pure]
        public static IEnumerable<EitherOrBoth<TLeft, TRight>> ZipLongest<TLeft, TRight>(this IEnumerable<TLeft> left, IEnumerable<TRight> right)
            where TLeft : notnull
            where TRight : notnull
            => left.ZipLongest(right, Identity);

        /// <summary>
        /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
        /// </summary>
        /// <param name="left">The left sequence to merge.</param>
        /// <typeparam name="TLeft">Type of the elements in <paramref name="left"/> sequence.</typeparam>
        /// <param name="right">The right sequence to merge.</param>
        /// <typeparam name="TRight">Type of the elements in <paramref name="right"/> sequence.</typeparam>
        /// <typeparam name="TResult">The return type of the result selector function.</typeparam>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
        /// <returns>A sequence that contains merged elements of two input sequences.</returns>
        [Pure]
        public static IEnumerable<TResult> ZipLongest<TLeft, TRight, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<EitherOrBoth<TLeft, TRight>, TResult> resultSelector)
            where TLeft : notnull
            where TRight : notnull
        {
            using var leftEnumerator = left.GetEnumerator();
            using var rightEnumerator = right.GetEnumerator();

            while (EitherOrBoth.FromOptions(ReadNext(leftEnumerator), ReadNext(rightEnumerator)).TryGetValue(out var value))
            {
                yield return resultSelector(value);
            }
        }

        private static Option<TSource> ReadNext<TSource>(IEnumerator<TSource> enumerator)
            where TSource : notnull
            => enumerator.MoveNext()
                ? enumerator.Current
                : Option<TSource>.None;
    }
}
