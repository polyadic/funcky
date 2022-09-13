using FsCheck;
using FsCheck.Xunit;
using Xunit.Sdk;

namespace Funcky.Test.Monads;

public sealed partial class EitherTest
{
    [Fact]
    public void InspectDoesNothingWhenEitherIsLeft()
    {
        var either = Either<string, int>.Left("foo");
        either.Inspect(_ => throw new XunitException("Side effect was unexpectedly called"));
    }

    [Fact]
    public void InspectCallsSideEffectWhenEitherIsRight()
    {
        const int value = 10;
        var either = Either<string>.Return(value);

        var sideEffect = Option<int>.None;
        either.Inspect(v => sideEffect = v);
        FunctionalAssert.Some(value, sideEffect);
    }

    [Theory]
    [MemberData(nameof(LeftAndRight))]
    public void InspectReturnsOriginalValue(Either<string, int> either)
    {
        Assert.Equal(either, either.Inspect(NoOperation));
    }

    [Fact]
    public void GivenARightCaseTheGetOrElseFuncIsNotExecuted()
    {
        var some = Either<int, string>.Right("Hello world!");

        _ = some.GetOrElse(SideEffect);

        static string SideEffect(int dummy)
            => throw new XunitException("This side effect should not happen!");
    }

    [Fact]
    public void GivenAnEitherTheGetOrElseShouldReturnTheArgumentOnlyInALeftCase()
    {
        var leftEither = Either<int, string>.Left(42);
        var rightEither = Either<int, string>.Right("Test");

        Assert.Equal("Test", rightEither.GetOrElse("Value"));
        Assert.Equal("Value", leftEither.GetOrElse("Value"));
        Assert.Equal("Test", rightEither.GetOrElse(HandleLeft));
        Assert.Equal("Value(42)", leftEither.GetOrElse(HandleLeft));

        string HandleLeft(int left) => $"Value({left})";
    }

    public sealed class SelectLeft
    {
        [Property]
        public Property ReturnsTheOriginalRightValue(int value)
        {
            var source = Either<Unit>.Return(value);
            var result = source.SelectLeft<Unit>(_ => throw new XunitException("This should never be called"));
            return (result.Match(left: False, right: r => r == value)).ToProperty();
        }

        [Property]
        public Property AppliesTheGivenSelectorToALeftValue(int value, Func<int, NonNull<string>> selector)
        {
            var source = Either<int, Unit>.Left(value);
            var result = source.SelectLeft(selector);
            return (result == Either<NonNull<string>, Unit>.Left(selector(value))).ToProperty();
        }
    }
}
