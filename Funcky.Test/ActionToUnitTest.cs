using Xunit;
using static Funcky.Functional;

namespace Funcky.Test
{
    public sealed class ActionToUnitTest
    {
        [Fact]
        public void OverloadResolutionWorks()
        {
            ActionToUnit(ActionWithNoParameters);
            ActionToUnit<int>(ActionWithOneParameter);
            ActionToUnit<int, int>(ActionWithTwoParameters);
        }

        private static void ActionWithNoParameters()
        {
        }

        private static void ActionWithOneParameter(int foo)
        {
        }

        private static void ActionWithTwoParameters(int foo, int bar)
        {
        }
    }
}