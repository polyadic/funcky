using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Funcky.Async.Extensions;
using Funcky.Monads;
using Xunit;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions
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
            => n % 2 == 0
                ? n * n
                : Option<int>.None;

        private static async IAsyncEnumerable<int> Ι(int n)
        {
            for (var ι = 0; ι < n; ι++)
            {
                yield return await new ValueTask<int>(ι);
            }
        }
    }
}
