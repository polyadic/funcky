using Xunit.Sdk;

namespace Funcky.Test.TestUtils;

internal static class AsyncAssert
{
    public static async Task Empty<TElement>(IAsyncEnumerable<TElement> asyncSequence)
    {
        var asyncEnumerator = asyncSequence.GetAsyncEnumerator();
        try
        {
            if (await asyncEnumerator.MoveNextAsync())
            {
                throw EmptyException.ForNonEmptyCollection(collection: await FormatCollectionStart(asyncSequence));
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
            throw SingleException.Empty(expected: null, collection: string.Empty);
        }

        var result = asyncEnumerator.Current;

        if (await asyncEnumerator.MoveNextAsync())
        {
            var actual = await MaterializeCollectionStart(asyncSequence);
            throw SingleException.MoreThanOne(expected: null, collection: FormatCollectionStart(actual), count: actual.Count, matchIndices: Array.Empty<int>());
        }

        return result;
    }

    public static async Task Equal<TElement>(IAsyncEnumerable<TElement> expectedResult, IAsyncEnumerable<TElement> actual)
        => Assert.Equal(await expectedResult.ToListAsync(), await actual.ToListAsync());

    private static async Task<IReadOnlyCollection<TElement>> MaterializeCollectionStart<TElement>(IAsyncEnumerable<TElement> asyncSequence)
    {
        // This should *ideally* be kept in sync with XUnit's `ArgumentFormatter.MAX_ENUMERABLE_LENGTH + 1` (which is private).
        const int maxEnumerableLength = 6;
        return await asyncSequence.Take(maxEnumerableLength).ToListAsync();
    }

    private static async Task<string> FormatCollectionStart<TElement>(IAsyncEnumerable<TElement> asyncSequence)
        => FormatCollectionStart(await MaterializeCollectionStart(asyncSequence));

    private static string FormatCollectionStart<TElement>(IEnumerable<TElement> sequence)
    {
        using var tracker = sequence.AsTracker();
        return tracker.FormatStart();
    }
}
