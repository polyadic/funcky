using Xunit;

namespace Funcky.Test.Extensions
{
    internal sealed class SkipOnMonoFact : FactAttribute
    {
        public SkipOnMonoFact()
        {
            if (IsRunningOnMono())
            {
                Skip = "This test does not work on Mono";
            }
        }

        private static bool IsRunningOnMono() => Type.GetType("Mono.Runtime") is not null;
    }
}
