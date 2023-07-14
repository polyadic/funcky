using Xunit.Sdk;

namespace Funcky.Test.Monads;

public sealed partial class ResultTest
{
    [Fact]
    public void InspectErrorDoesNothingWhenResultIsOk()
    {
        var result = Result.Ok("foo");
        result.InspectError(_ => throw new XunitException("Side effect was unexpectedly called"));
    }

    [Fact]
    public void InspectErrorCallsSideEffectWhenResultIsError()
    {
        var exception = new Exception("Bam!");
        var result = Result<string>.Error(exception);

        var sideEffect = Option<Exception>.None;
        result.InspectError(v => sideEffect = v);
        FunctionalAssert.Some(exception, sideEffect);
    }

    [Theory]
    [MemberData(nameof(OkAndError))]
    public void InspectErrorReturnsOriginalValue(Result<int> result)
    {
        Assert.Equal(result, result.InspectError(NoOperation));
    }
}
