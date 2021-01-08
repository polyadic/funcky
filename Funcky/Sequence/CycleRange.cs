using System.Collections.Generic;
using System.Linq;
using static Funcky.Functional;

namespace Funcky
{
    public static partial class Sequence
    {
        /// <summary>
        /// Generates a sequence that contains the same sequence of elements over and over again as an endless generator.
        /// </summary>
        /// <typeparam name="TItem">Type of the elements to be cycled.</typeparam>
        /// <param name="sequence">The sequence of elements which are cycled.</param>
        /// <returns>Returns an infinite IEnumerable repeating the same sequence of elements.</returns>
        public static IEnumerable<TItem> CycleRange<TItem>(IEnumerable<TItem> sequence)
            where TItem : notnull
            => Generate(Unit.Value, Identity)
                .SelectMany(_ => sequence);
    }
}
