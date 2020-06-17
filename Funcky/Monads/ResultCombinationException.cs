using System;
using System.Diagnostics.Contracts;

namespace Funcky.Monads
{
    public class ResultCombinationException : Exception
    {
        public ResultCombinationException(Exception first, Exception second)
        {
            First = first;
            Second = second;
        }

        [Pure]
        public Exception First { get; }

        [Pure]
        public Exception Second { get; }
    }
}
