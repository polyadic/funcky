using Xunit;

namespace Funcky.Xunit.Test.Extensions
{
    public sealed class ToTheoryDataExtensionTest
    {
        private const string StringValue = "Hello world!";
        private const int IntValue = 1337;

        [Theory]
        [MemberData(nameof(TheoryFromEnumerable))]
        public void GivenAnEnumerableEachElementCreatesATest(int index) =>
            Assert.InRange(index, 0, 9);

        public static TheoryData<int> TheoryFromEnumerable()
            => Enumerable.Range(0, 10).ToTheoryData();

        [Theory]
        [MemberData(nameof(TheoryFromRepeat))]
        public void GivenAnEnumerableOfTupleWeGetAllValues(int id, string value)
        {
            Assert.Equal(IntValue, id);
            Assert.Equal(StringValue, value);
        }

        public static TheoryData<int, string> TheoryFromRepeat()
            => Sequence.Return(Tuple.Create(IntValue, StringValue)).ToTheoryData();
    }
}
