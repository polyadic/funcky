using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Funcky.Monads;

namespace Funcky.Linq.Async
{
    public static partial class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Projects and filters an <see cref="IAsyncEnumerable{T}"/> at the same time.
        /// This is done by filtering out any empty <see cref="Option{T}"/> values returned by the <paramref name="selector"/>.
        /// </summary>
        [Pure]
        public static IAsyncEnumerable<TOutput> WhereSelect<TSource, TOutput>(this IAsyncEnumerable<TSource> inputs, Func<TSource, Option<TOutput>> selector)
            where TOutput : notnull
            => inputs.SelectMany(input => selector(input).ToAsyncEnumerable());

        /// <inheritdoc cref="WhereSelect{TSource,TOutput}"/>
        [Pure]
        public static IAsyncEnumerable<TOutput> WhereSelectAwait<TSource, TOutput>(this IAsyncEnumerable<TSource> inputs, Func<TSource, ValueTask<Option<TOutput>>> selector)
            where TOutput : notnull
            => inputs.SelectManyAwait(async input => (await selector(input)).ToAsyncEnumerable());

        /// <inheritdoc cref="WhereSelect{TSource,TOutput}"/>
        [Pure]
        public static IAsyncEnumerable<TOutput> WhereSelectAwaitWithCancellation<TSource, TOutput>(this IAsyncEnumerable<TSource> inputs, Func<TSource, CancellationToken, ValueTask<Option<TOutput>>> selector)
            where TOutput : notnull
            => inputs.SelectManyAwaitWithCancellation(async (input, cancellationToken) => (await selector(input, cancellationToken)).ToAsyncEnumerable());
    }
}
