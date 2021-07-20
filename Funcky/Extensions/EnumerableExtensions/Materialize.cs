using System.Collections.Immutable;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Materializes all the items of a lazy <see cref="IEnumerable{TItem}" />. If the underlying sequence is a collection type we do not actively enumerate them.
        /// </summary>
        /// <typeparam name="TItem">Type of the items in the source sequence.</typeparam>
        /// <param name="source">The source sequence can be any <see cref="IEnumerable{TItem}" />.</param>
        /// <returns>A collection of the enumerated items.</returns>
        public static IEnumerable<TItem> Materialize<TItem>(this IEnumerable<TItem> source)
            => source.Materialize(DefaultMaterialization);

        /// <summary>
        /// Materializes all the items of a lazy <see cref="IEnumerable{TItem}" />. If the underlying sequence is a collection type we do not actively enumerate them.
        /// Via the materialize function you can chose how the enumeration is done when it is needed.
        /// </summary>
        /// <typeparam name="TItem">Type of the items in the source sequence.</typeparam>
        /// <typeparam name="TMaterialization">The type of the materialization target.</typeparam>
        /// <param name="source">The source sequence can be any <see cref="IEnumerable{TItem}" />.</param>
        /// <param name="materialize">A function which materializes a given sequence into a collection.</param>
        /// <returns>A collection of the enumerated items.</returns>
        public static IEnumerable<TItem> Materialize<TItem, TMaterialization>(
            this IEnumerable<TItem> source,
            Func<IEnumerable<TItem>, TMaterialization> materialize)
            where TMaterialization : IEnumerable<TItem>
            => source is IReadOnlyCollection<TItem> or ICollection<TItem>
                ? source
                : materialize(source);

        private static ImmutableList<TItem> DefaultMaterialization<TItem>(IEnumerable<TItem> source)
            => source.ToImmutableList();
    }
}
