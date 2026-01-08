#if !NET48

using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Funcky.Test.Async.TestUtilities;

namespace Funcky.Test.Async;

public sealed class CycleTest
{
    [Property]
    public Property CycleCanProduceArbitraryManyItems(int value, PositiveInt arbitraryElements)
        => (AsyncSequence.CycleAsync(value).Take(arbitraryElements.Get).CountAsync().Result == arbitraryElements.Get)
            .ToProperty();

    [Property]
    public Property CycleRepeatsTheElementArbitraryManyTimes(int value, PositiveInt arbitraryElements)
        => AsyncSequence
            .CycleAsync(value)
            .IsSequenceRepeating(AsyncSequence.Return(value))
            .NTimes(arbitraryElements.Get)
            .Result
            .ToProperty();
}

#endif
