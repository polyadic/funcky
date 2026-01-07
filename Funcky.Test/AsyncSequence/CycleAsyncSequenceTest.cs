using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;

namespace Funcky.Test;

public sealed class CycleAsyncSequenceTest
{
    [Property]
    public Property CycleCanProduceArbitraryManyItems(int value, PositiveInt arbitraryElements)
        => (AsyncSequence.Cycle(value).Take(arbitraryElements.Get).CountAsync().Result == arbitraryElements.Get)
            .ToProperty();

    [Property]
    public Property CycleRepeatsTheElementArbitraryManyTimes(int value, PositiveInt arbitraryElements)
        => AsyncSequence
            .Cycle(value)
            .IsSequenceRepeating(AsyncSequence.Return(value))
            .NTimes(arbitraryElements.Get)
            .Result
            .ToProperty();
}
