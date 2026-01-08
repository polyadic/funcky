using static Funcky.Discard;

namespace Funcky.Test.Async.Extensions.AsyncEnumerableExtensions;

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
    public async Task WhereSelectReceivesTheSourceElementsIndex()
    {
        const int count = 6;
        var expectedSequence = Enumerable.Range(0, count: count);
        var units = AsyncSequence.CycleAsync(__).Take(count);
        var indexes = units.WhereSelect((_, index) => Option.Some(index));
        Assert.Equal(expectedSequence, await indexes.ToListAsync());
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
