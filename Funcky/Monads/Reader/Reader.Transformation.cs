using System.Collections.Generic;
using System.Linq;

namespace Funcky.Monads
{
    public static partial class ReaderExtensions
    {
        public static Reader<TEnvironment, IEnumerable<TElement>> Sequence<TEnvironment, TElement>(this IEnumerable<Reader<TEnvironment, TElement>> sequence)
            => environment
                => from element in sequence
                   select element(environment);
    }
}
