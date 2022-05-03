using FsCheck;
using FsCheck.Xunit;

namespace Funcky.Test.Monads;

public sealed partial class EitherTest
{
    [Property]
    public Property SequencingOptionTwiceReturnsOriginalValue(Either<int, Option<int>> option)
    {
        return (option == option.Sequence().Sequence()).ToProperty();
    }

    [Property]
    public Property SequencingResultTwiceReturnsOriginalValue(Either<int, Result<int>> option)
    {
        return (option == option.Sequence().Sequence()).ToProperty();
    }
}
