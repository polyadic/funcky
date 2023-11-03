using System.Collections.Immutable;

namespace Funcky.Test;

public sealed class UpCastTest
{
    [Fact]
    public void UpCastsReferenceTypeToObject()
    {
        const string arbitraryStringValue = "string";

        Option<object> option = UpCast<object>.From(Option.Return(arbitraryStringValue));
        FunctionalAssert.Some(arbitraryStringValue, option);

        Either<Unit, object> either = UpCast<object>.From(Either<Unit>.Return(arbitraryStringValue));
        FunctionalAssert.Right(arbitraryStringValue, either);

        Result<object> result = UpCast<object>.From(Result.Return(arbitraryStringValue));
        FunctionalAssert.Ok(arbitraryStringValue, result);

        Lazy<object> lazy = UpCast<object>.From(Lazy.Return(arbitraryStringValue));
        Assert.Equal(arbitraryStringValue, lazy.Value);
    }

    [Fact]
    public void UpCastsValueTypeToObject()
    {
        const int arbitraryIntegerValue = 10;

        Option<object> option = UpCast<object>.From(Option.Return(arbitraryIntegerValue));
        FunctionalAssert.Some(arbitraryIntegerValue, option);

        Either<Unit, object> either = UpCast<object>.From(Either<Unit>.Return(arbitraryIntegerValue));
        FunctionalAssert.Right(arbitraryIntegerValue, either);

        Result<object> result = UpCast<object>.From(Result.Return(arbitraryIntegerValue));
        FunctionalAssert.Ok(arbitraryIntegerValue, result);

        Lazy<object> lazy = UpCast<object>.From(Lazy.Return(arbitraryIntegerValue));
        Assert.Equal(arbitraryIntegerValue, lazy.Value);
    }

    [Fact]
    public void UpCastsToInterface()
    {
        var arbitraryImplementationValue = ImmutableArray<string>.Empty;

        Option<IEnumerable<string>> option = UpCast<IEnumerable<string>>.From(Option.Return(arbitraryImplementationValue));
        FunctionalAssert.Some(arbitraryImplementationValue, option);

        Either<Unit, IEnumerable<string>> either = UpCast<IEnumerable<string>>.From(Either<Unit>.Return(arbitraryImplementationValue));
        FunctionalAssert.Right(arbitraryImplementationValue, either);

        Result<IEnumerable<string>> result = UpCast<IEnumerable<string>>.From(Result.Return(arbitraryImplementationValue));
        FunctionalAssert.Ok(arbitraryImplementationValue, result);

        Lazy<IEnumerable<string>> lazy = UpCast<IEnumerable<string>>.From(Lazy.Return(arbitraryImplementationValue));
        Assert.Equal(arbitraryImplementationValue, lazy.Value);
    }
}
