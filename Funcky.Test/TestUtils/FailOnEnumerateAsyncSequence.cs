using System.Collections.Generic;
using System.Threading;
using Xunit.Sdk;

namespace Funcky.Test.TestUtils
{
    public class FailOnEnumerateAsyncSequence<T> : IAsyncEnumerable<T>
    {
        #pragma warning disable 1998,162
        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            throw new XunitException("Sequence was unexpectedly enumerated.");
            yield break;
        }
        #pragma warning restore 1998,162
    }
}
