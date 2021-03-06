using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Funcky.Internal;
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
            => AsyncEnumerable.Create(cancellationToken => WhereSelectAwaitWithCancellationInternal(source, selector, cancellationToken));

        #pragma warning disable 8425
        private static async IAsyncEnumerator<TOutput> WhereSelectAwaitWithCancellationInternal<TSource, TOutput>(
            IAsyncEnumerable<TSource> source,
            Func<TSource, CancellationToken, ValueTask<Option<TOutput>>> selector,
            CancellationToken cancellationToken)
            where TOutput : notnull
        {
            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                var projectedItem = await selector(item, cancellationToken);
                foreach (var value in projectedItem.ToEnumerable())
                {
                    yield return value;
                }
            }
        }
        #pragma warning restore 8425
    }
}
