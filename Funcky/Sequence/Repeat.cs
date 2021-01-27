using System;
using System.Collections.Generic;
using System.Linq;

namespace Funcky
{
    public static partial class Sequence
    {
        /// <summary>
        /// Generates a sequence of count elements from a generator source with state.
        /// </summary>
        /// <param name="count">The number of times to generate a value from the sequence.</param>
        /// <param name="generator">generator function.</param>
        /// <returns>Returns an IEnumerable with exactly count elements generated from the generator.</returns>
        public static IEnumerable<TItem> Repeat<TItem>(int count, Func<TItem> generator)
            where TItem : notnull
            => Enumerable
                .Repeat(Unit.Value, count)
                .Select(_ => generator());
    }
}
