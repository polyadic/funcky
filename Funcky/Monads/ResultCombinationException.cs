using System;


namespace Funcky.Monads
{
    public class ResultCombinationException : Exception
    {
        public ResultCombinationException(Exception first, Exception second)
        {
            First = first;
            Second = second;
        }

        public Exception First { get; }

        public Exception Second { get; }
    }
}