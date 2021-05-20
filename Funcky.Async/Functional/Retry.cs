using System;
using System.Collections.Generic;
using System.Linq;
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
        public static async ValueTask<TResult> RetryAsync<TResult>(Func<ValueTask<Option<TResult>>> producer)
            where TResult : notnull
            => await (await producer()).Match(
                none: (Func<ValueTask<TResult>>)(async () => await RetryAsync(producer)),
                some: result => new ValueTask<TResult>(result));

        public static async ValueTask<Option<TResult>> RetryAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, IRetryPolicy retryPolicy)
            where TResult : notnull
            => await AsyncEnumerable
                .Repeat(await producer(), FirstTry)
                .Concat(TailRetriesAsync(producer, retryPolicy))
                .WhereSelect(Identity)
                .FirstOrNoneAsync();

        private static IAsyncEnumerable<Option<TResult>> TailRetriesAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, IRetryPolicy retryPolicy)
            where TResult : notnull
            => Retries(retryPolicy)
                .SelectAwait(ProduceDelayedAsync(producer, retryPolicy));

        private static IAsyncEnumerable<int> Retries(IRetryPolicy retryPolicy)
            => AsyncEnumerable.Range(0, retryPolicy.MaxRetries);

        private static Func<int, ValueTask<Option<TResult>>> ProduceDelayedAsync<TResult>(Func<ValueTask<Option<TResult>>> producer, IRetryPolicy retryPolicy)
            where TResult : notnull
            => async retryCount =>
            {
                await Task.Delay(retryPolicy.Duration(retryCount));
                return await producer();
            };
    }
}
