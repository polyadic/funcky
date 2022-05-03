using FsCheck;
using FsCheck.Xunit;

namespace Funcky.Test.Monads;

public sealed partial class ResultTest
{
    [Property]
    public Property SequencingOptionTwiceReturnsOriginalValue(Result<Option<int>> option)
    {
        return (option == option.Sequence().Sequence()).ToProperty();
    }

    [Property]
    public Property SequencingEitherTwiceReturnsOriginalValue(Result<Either<int, int>> option)
    {
        return (option == option.Sequence().Sequence()).ToProperty();
    }
}
