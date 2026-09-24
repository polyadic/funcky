using static Funcky.Discard;

namespace Funcky.Test.FunctionalClass;

public class ApplyTest
{
    [Fact]
    public void CanApplyParametersInAnyOrder()
    {
        var func = Linear0;
        var f1 = Fn(Linear0).Apply(__, __, 10);
        var f2 = func.Apply(2, __, 7);
        var f3 = Apply(Fn(Linear0), 42, __, __);
        Assert.Equal(Linear0(10, 2, 10), f1(10, 2));
        Assert.Equal(Linear0(2, 10, 7), f2(10));
        Assert.Equal(Linear0(42, 10, 2), f3(10, 2));
    }

    private static int Linear0(int a, int b, int c)
        => a + (b * c);
}
