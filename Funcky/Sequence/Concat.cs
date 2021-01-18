using System.Collections.Generic;
using System.Linq;
using static Funcky.Functional;

namespace Funcky
{
    public static partial class Sequence
    {
        public static IEnumerable<TSource> Concat<TSource>(params IEnumerable<TSource>[] sources) => Concat(sources.AsEnumerable());

        public static IEnumerable<TSource> Concat<TSource>(IEnumerable<IEnumerable<TSource>> sources) => sources.SelectMany(Identity);
    }
}
