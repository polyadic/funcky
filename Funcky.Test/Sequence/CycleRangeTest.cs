using System;
using System.Collections.Immutable;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;
using Xunit;

namespace Funcky.Test
{
    public sealed class CycleRangeTest
    {
        [Fact]
        public void CyclingAnEmptySetThrowsAnArgumentException()
            => Assert.Throws<ArgumentException>(() => Sequence.CycleRange(ImmutableList<string>.Empty));

        [Property]
        public Property CycleRangeCanProduceArbitraryManyItems(NonEmptySet<int> sequence, PositiveInt arbitraryElements)
            => (Sequence.CycleRange(sequence.Get).Take(arbitraryElements.Get).Count() == arbitraryElements.Get)
                .ToProperty();

        [Property]
        public Property CycleRangeRepeatsTheElementsArbitraryManyTimes(NonEmptySet<int> sequence, PositiveInt arbitraryElements)
            => Sequence
                .CycleRange(sequence.Get)
                .IsSequenceRepeating(sequence.Get)
                .NTimes(arbitraryElements.Get);
    }
}
