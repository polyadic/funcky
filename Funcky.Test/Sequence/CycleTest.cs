using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Funcky.Test.TestUtilities;

namespace Funcky.Test;

public sealed class CycleTest
{
    [Property]
    public Property CycleCanProduceArbitraryManyItems(int value, PositiveInt arbitraryElements)
        => (Enumerable.Cycle(value).Take(arbitraryElements.Get).Count() == arbitraryElements.Get)
            .ToProperty();

    [Property]
    public Property CycleRepeatsTheElementArbitraryManyTimes(int value, PositiveInt arbitraryElements)
        => Enumerable
            .Cycle(value)
            .IsSequenceRepeating(Enumerable.Return(value))
            .NTimes(arbitraryElements.Get)
            .ToProperty();
}
