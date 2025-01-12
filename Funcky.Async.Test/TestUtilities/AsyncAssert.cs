using Xunit.Sdk;

namespace Funcky.Async.Test.TestUtilities;

internal static class AsyncAssert
{
    public static async Task Empty<TElement>(IAsyncEnumerable<TElement> asyncSequence)
    {
        var asyncEnumerator = asyncSequence.GetAsyncEnumerator();
        try
        {
            if (await asyncEnumerator.MoveNextAsync())
            {
                var actual = await asyncSequence.ToListAsync();
                throw EmptyException.ForNonEmptyCollection(collection: "TODO");
            }
        }
        finally
        {
            await asyncEnumerator.DisposeAsync();
        }
    }

    public static async Task NotEmpty<TElement>(IAsyncEnumerable<TElement> asyncSequence)
    {
        var asyncEnumerator = asyncSequence.GetAsyncEnumerator();
        try
        {
            if (!await asyncEnumerator.MoveNextAsync())
            {
                throw NotEmptyException.ForNonEmptyCollection();
            }
        }
        finally
        {
            await asyncEnumerator.DisposeAsync();
        }
    }

    public static async Task Collection<TElement>(IAsyncEnumerable<TElement> asyncSequence, params Action<TElement>[] elementInspectors)
    {
        var elements = await asyncSequence.ToListAsync();
        Assert.Collection(elements, elementInspectors);
    }

    public static async Task<T> Single<T>(IAsyncEnumerable<T> asyncSequence)
    {
        await using var asyncEnumerator = asyncSequence.GetAsyncEnumerator();

        if (await asyncEnumerator.MoveNextAsync() is false)
        {
            throw SingleException.Empty(expected: null, collection: "TODO");
        }

        var result = asyncEnumerator.Current;

        if (await asyncEnumerator.MoveNextAsync())
        {
            var actual = await asyncSequence.ToListAsync();
            throw SingleException.MoreThanOne(expected: null, collection: "TODO", count: actual.Count, matchIndices: Array.Empty<int>());
        }

        return result;
    }

    public static async Task Equal<TElement>(IAsyncEnumerable<TElement> expectedResult, IAsyncEnumerable<TElement> actual)
        => Assert.Equal(await expectedResult.ToListAsync(), await actual.ToListAsync());
}
