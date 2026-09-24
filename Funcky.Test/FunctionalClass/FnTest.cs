namespace Funcky.Test.FunctionalClass;

public sealed class FnTest
{
    [Fact]
    public void FnHelpsToInferAMethodGroupsNaturalType()
    {
        var powCurried = Curry(Fn(Math.Pow));
        var powUncurried = Uncurry(Fn(CurriedPow));
        var flipped = Flip(Fn(Math.Pow));
    }

    private static Func<double, double> CurriedPow(double x) => y => Math.Pow(x, y);
}
