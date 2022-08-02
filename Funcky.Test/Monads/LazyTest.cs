using Funcky.FsCheck;

namespace Funcky.Test.Monads;

public sealed partial class LazyTest
{
    public LazyTest()
        => FunckyGenerators.Register();
}
