using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Funcky.Async.Test.TestUtilities
{
    public static class AsyncAssert
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
                if (asyncEnumerator is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
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
                if (asyncEnumerator is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
            }
        }
    }
}
