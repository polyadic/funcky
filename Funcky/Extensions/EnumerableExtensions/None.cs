namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// Determines whether no element exists or satisfies a condition.
        /// </summary>
        [Pure]
        public bool None(Func<TSource, bool> predicate)
            => !source.Any(predicate);

        /// <summary>
        /// Determines whether a sequence contains no elements.
        /// </summary>
        [Pure]
        public bool None()
            => !source.Any();
    }
}
