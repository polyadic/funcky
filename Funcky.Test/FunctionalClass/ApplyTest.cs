using static Funcky.Discard;

namespace Funcky.Test.Extensions.FuncExtensions;

public class ApplyTest
{
    [Fact]
    public void CanApplyParametersInAnyOrder()
    {
        var func = Linear0;

        var f1 = ((Func<int, int, int, int>)Linear0).Apply(__, __, 10);
        var f2 = func.Apply(2, __, 7);
        var f3 = Funcky.Extensions.FuncExtensions.Apply<int, int, int, int>(Linear0, __, __, 42);

        Assert.Equal(30, f1(10, 2));
        Assert.Equal(72, f2(10));
        Assert.Equal(30, f3(10, 2));
    }

    [Fact]
    public void Test2()
    {
    }

    private static int Linear0(int a, int b, int c)
        => a + (b * c);
}
