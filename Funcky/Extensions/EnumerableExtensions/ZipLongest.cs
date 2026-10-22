using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <param name="left">The left sequence to merge.</param>
    /// <typeparam name="TLeft">Type of the elements in <paramref name="left"/> sequence.</typeparam>
    extension<TLeft>(IEnumerable<TLeft> left)
        where TLeft : notnull
    {
        /// <summary>
        /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
        /// </summary>
        /// <param name="right">The right sequence to merge.</param>
        /// <typeparam name="TRight">Type of the elements in <paramref name="right"/> sequence.</typeparam>
        /// <returns>A sequence that contains merged elements of two input sequences.</returns>
        [Pure]
        public IEnumerable<EitherOrBoth<TLeft, TRight>> ZipLongest<TRight>(IEnumerable<TRight> right)
            where TRight : notnull
            => left.ZipLongest(right, Identity);

        /// <summary>
        /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
        /// </summary>
        /// <param name="right">The right sequence to merge.</param>
        /// <typeparam name="TRight">Type of the elements in <paramref name="right"/> sequence.</typeparam>
        /// <typeparam name="TResult">The return type of the result selector function.</typeparam>
        /// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
        /// <returns>A sequence that contains merged elements of two input sequences.</returns>
        [Pure]
        public IEnumerable<TResult> ZipLongest<TRight, TResult>(IEnumerable<TRight> right, Func<EitherOrBoth<TLeft, TRight>, TResult> resultSelector)
            where TRight : notnull
            => ZipLongestIterator(ArgumentValidator.NotNull(left), ArgumentValidator.NotNull(right), ArgumentValidator.NotNull(resultSelector));
    }

    private static IEnumerable<TResult> ZipLongestIterator<TLeft, TRight, TResult>(IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<EitherOrBoth<TLeft, TRight>, TResult> resultSelector)
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
