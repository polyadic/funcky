namespace Funcky.Test;

public sealed class DownCastTest
{
    [Fact]
    public void DownCastToObjectOfRuntimeTypeReturnsSomeOfTheRequestedType()
    {
        var option = Option.Some<object>("Hello world!");

        Assert.Some("Hello world!", DownCast<string>.From(option));
    }

    [Fact]
    public void DownCastToWrongObjectOfRuntimeTypeReturnsNone()
    {
        var option = Option.Some(new object());

        Assert.None(DownCast<string>.From(option));
    }

    [Fact]
    public void DownCastFromANoneAlwaysGivesNone()
    {
        var option = Option<object>.None;

        Assert.None(DownCast<string>.From(option));
    }

    [Fact]
    public void DownCastToObjectOfRuntimeTypeReturnsRightOfTheRequestedType()
    {
        var either = Either<string, object>.Right("Hello world!");

        Assert.Right("Hello world!", DownCast<string>.From(either, () => "failed cast!"));
    }

    [Fact]
    public void DownCastToWrongObjectOfRuntimeTypeReturnsLeft()
    {
        var either = Either<string, object>.Right(new object());

        Assert.Left("failed cast!", DownCast<string>.From(either, () => "failed cast!"));
    }

    [Fact]
    public void DownCastFromALeftAlwaysGivesTheOldLeft()
    {
        var either = Either<string, object>.Left("initial left state!");

        Assert.Left("initial left state!", DownCast<string>.From(either, () => "failed cast!"));
    }

    [Fact]
    public void DownCastToObjectOfRuntimeTypeReturnsOkOfTheRequestedType()
    {
        var result = Result.Ok<object>("Hello world!");

        Assert.Ok("Hello world!", DownCast<string>.From(result));
    }

    [Fact]
    public void DownCastToWrongObjectOfRuntimeTypeReturnsError()
    {
        var result = Result.Ok(new object());

        var exception = Assert.Error(DownCast<string>.From(result));
        Assert.IsType<InvalidCastException>(exception);
    }

    [Fact]
    public void DownCastFromAnErrorAlwaysGivesTheOldError()
    {
        var result = Result<object>.Error(new FileNotFoundException());

        var exception = Assert.Error(DownCast<string>.From(result));
        Assert.IsType<FileNotFoundException>(exception);
    }
}
