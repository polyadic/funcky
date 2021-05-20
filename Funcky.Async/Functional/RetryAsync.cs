using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Funcky.Async.Extensions;
using Funcky.Monads;
using Funcky.RetryPolicies;
using static Funcky.Functional;

namespace Funcky.Async
{
    public static partial class Functional
    {
        private const int FirstTry = 1;

        /// <summary>
        /// Calls the given <paramref name="producer"/> over and over until it returns a value.
        /// </summary>
        public static async ValueTask<TResult> RetryAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, CancellationToken cancellationToken = default)
            where TResult : notnull
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await (await producer()).Match(
                none: (Func<ValueTask<TResult>>)(async () => await RetryAsync(producer, cancellationToken)),
                some: result => new ValueTask<TResult>(result));
        }

        public static async ValueTask<Option<TResult>> RetryAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, IRetryPolicy retryPolicy, CancellationToken cancellationToken = default)
            where TResult : notnull
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await AsyncEnumerable
                .Repeat(await producer(), FirstTry)
                .Concat(TailRetriesAsync(producer, retryPolicy, cancellationToken))
                .WhereSelect(Identity)
                .FirstOrNoneAsync(cancellationToken);
        }

        private static IAsyncEnumerable<Option<TResult>> TailRetriesAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, IRetryPolicy retryPolicy, CancellationToken cancellationToken)
            where TResult : notnull
            => Retries(retryPolicy)
                .SelectAwait(ProduceDelayedAsync(producer, retryPolicy, cancellationToken));

        private static IAsyncEnumerable<int> Retries(IRetryPolicy retryPolicy)
            => AsyncEnumerable.Range(0, retryPolicy.MaxRetries);

        private static Func<int, ValueTask<Option<TResult>>> ProduceDelayedAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, IRetryPolicy retryPolicy, CancellationToken cancellationToken)
            where TResult : notnull
            => async retryCount =>
            {
                await Task.Delay(retryPolicy.Duration(retryCount), cancellationToken);
                return await producer();
            };
    }
}
