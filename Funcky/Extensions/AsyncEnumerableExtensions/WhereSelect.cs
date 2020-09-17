using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Projects and filters an <see cref="IAsyncEnumerable{T}"/> at the same time.
        /// This is done by filtering out any empty <see cref="Option{T}"/> values returned by the <paramref name="selector"/>.
        /// </summary>
        [Pure]
        public static IAsyncEnumerable<TOutput> WhereSelect<TSource, TOutput>(this IAsyncEnumerable<TSource> source, Func<TSource, Option<TOutput>> selector)
            where TOutput : notnull
            => source.WhereSelectAwaitWithCancellation((item, _) => new ValueTask<Option<TOutput>>(selector(item)));

        /// <inheritdoc cref="WhereSelect{TSource,TOutput}"/>
        [Pure]
        public static IAsyncEnumerable<TOutput> WhereSelectAwait<TSource, TOutput>(this IAsyncEnumerable<TSource> source, Func<TSource, ValueTask<Option<TOutput>>> selector)
            where TOutput : notnull
            => source.WhereSelectAwaitWithCancellation((item, _) => selector(item));

        /// <inheritdoc cref="WhereSelect{TSource,TOutput}"/>
        [Pure]
        public static IAsyncEnumerable<TOutput> WhereSelectAwaitWithCancellation<TSource, TOutput>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<Option<TOutput>>> selector)
            where TOutput : notnull
            => new ProjectingAsyncEnumerable<TSource, TOutput>(
                source,
                (enumerator, cancellationToken) => new WhereSelectAwaitAsyncEnumerator<TSource, TOutput>(
                    enumerator, selector, cancellationToken));

        private sealed class WhereSelectAwaitAsyncEnumerator<TSource, TOutput> : IAsyncEnumerator<TOutput>
            where TOutput : notnull
        {
            private readonly IAsyncEnumerator<TSource> _source;
            private readonly Func<TSource, CancellationToken, ValueTask<Option<TOutput>>> _selector;
            private readonly CancellationToken _cancellationToken;

            public WhereSelectAwaitAsyncEnumerator(
                IAsyncEnumerator<TSource> source,
                Func<TSource, CancellationToken, ValueTask<Option<TOutput>>> selector,
                CancellationToken cancellationToken)
            {
                _source = source;
                _selector = selector;
                _cancellationToken = cancellationToken;
            }

            public TOutput Current { get; private set; } = default!;

            [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP007", Justification = "Enumerator holds ownership")]
            public ValueTask DisposeAsync() => _source.DisposeAsync();

            public async ValueTask<bool> MoveNextAsync()
            {
                _cancellationToken.ThrowIfCancellationRequested();

                while (await _source.MoveNextAsync())
                {
                    _cancellationToken.ThrowIfCancellationRequested();

                    var item = await _selector(_source.Current, _cancellationToken);
                    foreach (var value in item.ToEnumerable())
                    {
                        Current = value;
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
