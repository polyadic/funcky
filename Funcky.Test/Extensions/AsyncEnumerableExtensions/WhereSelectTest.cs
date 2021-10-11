namespace Funcky.Test.Extensions.AsyncEnumerableExtensions
{
    public sealed class WhereSelectTest
    {
        [Fact]
        public async Task WhereSelectRetainsOnlySomeValues()
        {
            var expectedSquares = new[] { 0, 4, 16, 36, 64 };
            var squares = await Ι(10).WhereSelect(SquareEvenNumbers).ToListAsync();
            Assert.Equal(expectedSquares, squares);
        }

        [Fact]
        public async Task WhereSelectWithoutAnArgumentFiltersEmptyValuesFromIAsyncEnumerable()
        {
            var withNone = new List<Option<int>> { Option<int>.None(), 0, Option<int>.None(), 1337, Option<int>.None(), 42, 0, 12, Option<int>.None(), 1 }.ToAsyncEnumerable();
            var expectedResult = new List<int> { 0, 1337, 42, 0, 12, 1 };

            Assert.Equal(expectedResult, await withNone.WhereSelect().ToListAsync());
        }

        private static Option<int> SquareEvenNumbers(int n)
            => n % 2 == 0
                ? n * n
                : Option<int>.None();

        private static async IAsyncEnumerable<int> Ι(int n)
        {
            for (var ι = 0; ι < n; ι++)
            {
                yield return await new ValueTask<int>(ι);
            }
        }
    }
}
