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

        private static Option<int> SquareEvenNumbers(int n)
            => Option.FromBoolean(n % 2 == 0, n * n);

        private static async IAsyncEnumerable<int> Ι(int n)
        {
            for (var ι = 0; ι < n; ι++)
            {
                yield return await new ValueTask<int>(ι);
            }
        }
    }
}
