using Funcky.Monads;
using Xunit;

namespace Funcky.Test.Monads
{
    public sealed partial class OptionTest
    {
        [Theory]
        [MemberData(nameof(OptionValues))]
        public void AssociativityHolds(Option<int> input)
        {
            static Option<int> MultiplyByTen(int x) => Option.Some(x * 10);
            static Option<int> AddTwo(int x) => Option.Some(x + 2);
            static Option<int> MultiplyByTenAndAddTwo(int x) => MultiplyByTen(x).SelectMany(AddTwo);

            Assert.Equal(
                input.SelectMany(MultiplyByTen).SelectMany(AddTwo),
                input.SelectMany(MultiplyByTenAndAddTwo));
        }

        [Theory]
        [MemberData(nameof(OptionValues))]
        public void RightIdentityHolds(Option<int> input)
        {
            Assert.Equal(input.SelectMany(Option.Some), input);
        }

        [Fact]
        public void LeftIdentityHolds()
        {
            static Option<int> MultiplyByTen(int x) => Option.Some(x * 10);

            const int input = 10;
            Assert.Equal(
                Option.Some(input).SelectMany(MultiplyByTen),
                MultiplyByTen(input));
        }

        private static TheoryData<Option<int>> OptionValues()
            => new()
            {
                Option.Some(10),
                Option<int>.None,
            };
    }
}
