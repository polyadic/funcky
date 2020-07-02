using Xunit.Sdk;

namespace Funcky.Xunit
{
    internal class UnmatchedException : XunitException
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
