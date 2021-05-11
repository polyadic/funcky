using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funcky.Monads.Reader
{
    public static partial class ReaderExtensions
    {
        public static Reader<TEnvironment, IEnumerable<TElement>> Sequence<TEnvironment, TElement>(this IEnumerable<Reader<TEnvironment, TElement>> sequence)
            => environment
                => from element in sequence
                   select element(environment);
    }
}
