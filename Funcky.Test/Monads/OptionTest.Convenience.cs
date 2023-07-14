using Xunit.Sdk;

namespace Funcky.Test.Monads;

public sealed partial class OptionTest
{
    [Fact]
    public void InspectNoneDoesNothingWhenOptionIsNone()
    {
        var option = Option.Some(10);
        option.InspectNone(() => throw new XunitException("Side effect was unexpectedly called"));
    }

    [Fact]
    public void InspectNoneCallsSideEffectWhenOptionIsNone()
    {
        var option = Option<int>.None;

        var sideEffect = false;
        option.InspectNone(() => sideEffect = true);
        Assert.True(sideEffect);
    }

    [Theory]
    [MemberData(nameof(SomeAndNone))]
    public void InspectLeftReturnsOriginalValue(Option<int> option)
    {
        Assert.Equal(option, option.InspectNone(NoOperation));
    }

    public static TheoryData<Option<int>> SomeAndNone()
        => new()
        {
            Option<int>.None,
            Option.Some(42),
        };
}
