using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Funcky.Extensions;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public class ShuffleTest
    {
        [Property]
        public Property AShuffleHasTheSameElementsAsTheSource(List<int> source)
            => source
                .Shuffle()
                .All(source.Contains)
                .ToProperty();

        [Property]
        public Property AShuffleHasTheSameLengthAsTheSource(List<int> source)
            => (source.Shuffle().Count() == source.Count)
                .ToProperty();
    }
}
