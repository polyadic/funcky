using System.Collections.Immutable;
using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;

namespace Funcky.Test
{
    public sealed class CycleTest
    {
        [Property]
        public Property CycleCanProduceArbitraryManyItems(int value, PositiveInt arbitraryElements)
            => (Sequence.Cycle(value).Take(arbitraryElements.Get).Count() == arbitraryElements.Get)
                .ToProperty();

        [Property]
        public Property CycleRepeatsTheElementArbitraryManyTimes(int value, PositiveInt arbitraryElements)
            => Sequence
                .Cycle(value)
                .IsSequenceRepeating(ImmutableList.Create(value))
                .NTimes(arbitraryElements.Get);
    }
}
