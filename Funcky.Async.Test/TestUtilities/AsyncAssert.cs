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
                throw new EmptyException(await asyncSequence.ToListAsync());
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
                throw new NotEmptyException();
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
        var elementInspectorsLength = elementInspectors.Length;
        var elementsLength = elements.Count;

        if (elementInspectorsLength != elementsLength)
        {
            throw new CollectionException(asyncSequence.ToListAsync(), elementInspectorsLength, elementsLength);
        }

        foreach (var ((elementInspector, element), indexFailurePoint) in elementInspectors.Zip(elements).WithIndex())
        {
            try
            {
                elementInspector(element);
            }
            catch (Exception ex)
            {
                throw new CollectionException(asyncSequence.ToListAsync(), elementInspectorsLength, elementsLength, indexFailurePoint, ex);
            }
        }
    }

    public static async Task Equal<TElement>(IAsyncEnumerable<TElement> expectedResult, IAsyncEnumerable<TElement> actual)
        => Assert.Equal(await expectedResult.ToListAsync(), await actual.ToListAsync());
}
