using FsCheck;
using Funcky.FsCheck;
using Funcky.Test.TestUtilities;

namespace Funcky.Test.Monads;

public sealed partial class ReaderTest
{
    [FunckyProperty]
    public Property FlattenReaderReaderIsReader(string environment, Reader<string, string> input)
        => CheckAssert.Equal(input, Reader<string>.Return(input).Flatten(), environment);
}
