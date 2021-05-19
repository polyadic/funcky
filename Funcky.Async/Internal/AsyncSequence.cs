using System.Collections.Generic;
using System.Linq;

namespace Funcky.Async.Internal
{
    internal static class AsyncSequence
    {
        public static IAsyncEnumerable<TResult> Return<TResult>(TResult element)
            => AsyncEnumerable.Repeat(element, 1);
    }
}
