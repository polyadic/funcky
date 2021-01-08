using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Funcky.Test
{
    public class RepeatRangeTest
    {
        [Fact]
        public void NextTest()
        {
            List<int> list = new () { 1, 1337, 42 };

            var sequence = Sequence.RepeatRange(list, 3);

            Assert.Equal(9, sequence.Count());
        }
    }
}
