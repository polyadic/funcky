using Funcky.Test.TestUtils;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    public sealed class WhereSelectTest
    {
        [Fact]
        public void WhereSelectNullIsEnumeratedLazily()
        {
            var doNotEnumerate = new FailOnEnumerationSequence<object>();

            _ = doNotEnumerate.WhereSelect(_ => Option<object>.None());
        }

        [Fact]
        public void WhereSelectFiltersEmptyValues()
        {
            var input = Enumerable.Range(0, 10);
            var expectedResult = new[] { 0, 4, 16, 36, 64 };
            var result = input.WhereSelect(SquareEvenNumbers);
            Assert.Equal(expectedResult, result);
        }

        private static Option<int> SquareEvenNumbers(int number)
            => Option.FromBoolean(IsEven(number), number * number);

        private static bool IsEven(int number)
            => number % 2 == 0;
    }
}
