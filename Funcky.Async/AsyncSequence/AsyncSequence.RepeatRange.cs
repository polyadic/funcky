namespace Funcky
{
    public static partial class AsyncSequence
    {
        /// <summary>
        /// Generates a sequence that contains the same sequence of elements the given number of times.
        /// </summary>
        /// <typeparam name="TItem">Type of the elements to be repeated.</typeparam>
        /// <param name="sequence">The sequence of elements to be repeated.</param>
        /// <param name="count">The number of times to repeat the value in the generated sequence.</param>
        /// <returns>Returns an infinite IEnumerable cycling through the same elements.</returns>
        [Pure]
        public static IAsyncEnumerable<TItem> RepeatRange<TItem>(IEnumerable<TItem> sequence, int count)
            where TItem : notnull
            => AsyncEnumerable
                .Repeat(Unit.Value, count)
                .SelectMany(_ => sequence.ToAsyncEnumerable());

        /// <summary>
        /// Generates a sequence that contains the same sequence of elements the given number of times.
        /// </summary>
        /// <typeparam name="TItem">Type of the elements to be repeated.</typeparam>
        /// <param name="sequence">The sequence of elements to be repeated.</param>
        /// <param name="count">The number of times to repeat the value in the generated sequence.</param>
        /// <returns>Returns an infinite IEnumerable cycling through the same elements.</returns>
        [Pure]
        public static IAsyncEnumerable<TItem> RepeatRange<TItem>(IAsyncEnumerable<TItem> sequence, int count)
            where TItem : notnull
            => AsyncEnumerable
                .Repeat(Unit.Value, count)
                .SelectMany(_ => sequence);
    }
}
