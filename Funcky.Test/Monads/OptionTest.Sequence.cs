using FsCheck;
using FsCheck.Xunit;

namespace Funcky.Test.Monads;

public sealed partial class OptionTest
{
    [Property]
    public Property SequencingEitherTwiceReturnsOriginalValue(Option<Either<int, int>> option)
    {
        return (option == option.Sequence().Sequence()).ToProperty();
    }

    [Property]
    public Property SequencingResultTwiceReturnsOriginalValue(Option<Result<int>> option)
    {
        return (option == option.Sequence().Sequence()).ToProperty();
    }
}
