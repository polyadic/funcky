namespace Funcky.Test;

public sealed class DownCastTest
{
    [Fact]
    public void DownCastToObjectOfRuntimeTypeReturnsSomeOfTheRequestedType()
    {
        var option = Option.Some<object>("Hello world!");

        FunctionalAssert.Some("Hello world!", DownCast<string>.From(option));
    }

    [Fact]
    public void DownCastToWrongObjectOfRuntimeTypeReturnsNone()
    {
        var option = Option.Some(new object());

        FunctionalAssert.None(DownCast<string>.From(option));
    }

    [Fact]
    public void DownCastFromANoneAlwaysGivesNone()
    {
        var option = Option<object>.None;

        FunctionalAssert.None(DownCast<string>.From(option));
    }

    [Fact]
    public void DownCastToObjectOfRuntimeTypeReturnsRightOfTheRequestedType()
    {
        var either = Either<string, object>.Right("Hello world!");

        FunctionalAssert.Right("Hello world!", DownCast<string>.From(either, () => "failed cast!"));
    }

    [Fact]
    public void DownCastToWrongObjectOfRuntimeTypeReturnsLeft()
    {
        var either = Either<string, object>.Right(new object());

        FunctionalAssert.Left("failed cast!", DownCast<string>.From(either, () => "failed cast!"));
    }

    [Fact]
    public void DownCastFromALeftAlwaysGivesTheOldLeft()
    {
        var either = Either<string, object>.Left("initial left state!");

        FunctionalAssert.Left("initial left state!", DownCast<string>.From(either, () => "failed cast!"));
    }

    [Fact]
    public void DownCastToObjectOfRuntimeTypeReturnsOkOfTheRequestedType()
    {
        var result = Result.Ok<object>("Hello world!");

        FunctionalAssert.Ok("Hello world!", DownCast<string>.From(result));
    }

    [Fact]
    public void DownCastToWrongObjectOfRuntimeTypeReturnsError()
    {
        var result = Result.Ok(new object());

        var exception = FunctionalAssert.Error(DownCast<string>.From(result));
        Assert.IsType<InvalidCastException>(exception);
    }

    [Fact]
    public void DownCastFromAnErrorAlwaysGivesTheOldError()
    {
        var result = Result<object>.Error(new FileNotFoundException());

        var exception = FunctionalAssert.Error(DownCast<string>.From(result));
        Assert.IsType<FileNotFoundException>(exception);
    }
}
