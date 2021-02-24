using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Materializes all the items of a lazy IEnumerable{TItem}. If the underlying sequence is a collection type we do not actively enumerate them.
        /// </summary>
        /// <typeparam name="TItem">Type of the items in the source sequence.</typeparam>
        /// <param name="source">The source sequence can be any IEnumerable{TItem}.</param>
        /// <returns>A collection of the enumerated items.</returns>
        public static IEnumerable<TItem> Materialize<TItem>(this IEnumerable<TItem> source)
            => source.Materialize(DefaultMaterialization);

        /// <summary>
        /// Materializes all the items of a lazy IEnumerable{TItem}. If the underlying sequence is a collection type we do not actively enumerate them.
        /// Via the materialize function you can chose how the enumeration is done when it is needed.
        /// </summary>
        /// <typeparam name="TItem">Type of the items in the source sequence.</typeparam>
        /// <typeparam name="TMaterialization">The type of the materialization target.</typeparam>
        /// <param name="source">The source sequence can be any IEnumerable{TItem}.</param>
        /// <param name="materialize">A function which materializes a given sequence into a collection.</param>
        /// <returns>A collection of the enumerated items.</returns>
        public static IEnumerable<TItem> Materialize<TItem, TMaterialization>(this IEnumerable<TItem> source, Func<IEnumerable<TItem>, TMaterialization> materialize)
            where TMaterialization : IEnumerable<TItem>
        {
            return source switch
            {
                List<TItem> => source,
                LinkedList<TItem> => source,
                HashSet<TItem> => source,
                SortedSet<TItem> => source,
                Stack<TItem> => source,
                Queue<TItem> => source,
                ImmutableList<TItem> => source,
                ImmutableArray<TItem> => source,
                ImmutableHashSet<TItem> => source,
                ImmutableSortedSet<TItem> => source,
                ImmutableStack<TItem> => source,
                ImmutableQueue<TItem> => source,
                IImmutableList<TItem> => source,
                IImmutableSet<TItem> => source,
                IImmutableStack<TItem> => source,
                IImmutableQueue<TItem> => source,
                IReadOnlyList<TItem> => source,
#if I_READ_ONLY_SET_SUPPORTED
                IReadOnlySet<TItem> => source,
#endif
                IReadOnlyCollection<TItem> => source,
                IList<TItem> => source,
                ISet<TItem> => source,
                ICollection<TItem> => source,
                _ => materialize(source),
            };
        }

        private static ImmutableList<TItem> DefaultMaterialization<TItem>(IEnumerable<TItem> source)
            => source.ToImmutableList();
    }
}
