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
    }
}
