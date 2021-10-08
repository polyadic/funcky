namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Filters out all the empty values from an IEnumerable&lt;Option&lt;T&gt;&gt; and therefore returns an <see cref="IEnumerable{TItem}"/>.
        /// </summary>
        [Pure]
        public static IEnumerable<TItem> WhereSelect<TItem>(this IEnumerable<Option<TItem>> sequence)
            where TItem : notnull
            => sequence.WhereSelect(Identity);

        /// <summary>
        /// Projects and filters an <see cref="IEnumerable{T}"/> at the same time.
        /// This is done by filtering out any empty <see cref="Option{T}"/> values returned by the <paramref name="selector"/>.
        /// </summary>
        [Pure]
        public static IEnumerable<TOutput> WhereSelect<TInput, TOutput>(this IEnumerable<TInput> inputs, Func<TInput, Option<TOutput>> selector)
            where TOutput : notnull
            => inputs.SelectMany(input => selector(input).ToEnumerable());
    }
}
