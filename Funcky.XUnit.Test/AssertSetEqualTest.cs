using System.Collections.Generic;
using System.Linq;
using Funcky.Xunit;
using Xunit;

namespace Funcky.XUnit.Test
{
    public class AssertSetEqualTest
    {
        [Fact]
        public void TwoEmptySequencesAreEqual()
        {
            var empty1 = Enumerable.Empty<int>();
            var empty2 = Enumerable.Empty<int>();

            FunctionalAssert.IsSetEqual(empty1, empty2);
        }

        [Fact]
        public void AnEmptySequenceAndOneWithAValueAreNotEqual()
        {
            var list = new List<int> { 0, 1, 42, 100, 1337 };
            var deque = new Queue<int>();
            deque.Enqueue(1337);
            deque.Enqueue(42);
            deque.Enqueue(0);
            deque.Enqueue(100);
            deque.Enqueue(21);

            Assert.Equal(list, deque, CollectionEquality<int>.UnorderedSequenceEquality);
            FunctionalAssert.IsSetEqual(list, deque);
        }
    }
}
