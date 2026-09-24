using FsCheck;
using Funcky.FsCheck;
using Funcky.Test.TestUtilities;

namespace Funcky.Test.Monads;

public sealed partial class LazyTest
{
    [FunckyProperty]
    public Property FlattenLazyLazyIsLazy(Lazy<string> input)
        => CheckAssert.Equal(Lazy.Return(input).Flatten(), input);
}
