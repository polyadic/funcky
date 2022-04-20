namespace Funcky.Test.FunctionalClass;

public sealed class ActionToUnitTest
{
    [Fact]
    public void OverloadResolutionWorks()
    {
        _ = ActionToUnit(ActionWithNoParameters);
        _ = ActionToUnit<int>(ActionWithOneParameter);
        _ = ActionToUnit<int, int>(ActionWithTwoParameters);
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
