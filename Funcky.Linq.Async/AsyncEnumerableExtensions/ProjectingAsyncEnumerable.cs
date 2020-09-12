using System;
using System.Collections.Generic;
using System.Threading;

namespace Funcky.Linq.Async
{
    internal sealed class ProjectingAsyncEnumerable<TSource, TOutput> : IAsyncEnumerable<TOutput>
    {
        private readonly IAsyncEnumerable<TSource> _source;
        private readonly Func<IAsyncEnumerator<TSource>, CancellationToken, IAsyncEnumerator<TOutput>> _createEnumerator;

        public ProjectingAsyncEnumerable(
            IAsyncEnumerable<TSource> source,
            Func<IAsyncEnumerator<TSource>, CancellationToken, IAsyncEnumerator<TOutput>> createEnumerator)
        {
            _source = source;
            _createEnumerator = createEnumerator;
        }

        public IAsyncEnumerator<TOutput> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => _createEnumerator(_source.GetAsyncEnumerator(cancellationToken), cancellationToken);
    }
}
