using System;

namespace Funcky.Test
{
    internal class UnmatchedException : Exception
    {
        public UnmatchedException()
            : base("Wrong pattern matched.")
        {
        }

        public UnmatchedException(string? unmatchedCase)
            : base($"Wrong pattern matched: the case '{unmatchedCase}' has been matched accidentally.")
        {
        }
    }
}
