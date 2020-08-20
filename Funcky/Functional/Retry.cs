using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Funcky.Monads;
using static System.Threading.Thread;

namespace Funcky
{
    public static partial class Functional
    {
        private const int FirstTry = 1;

        /// <summary>
        /// Calls the given <paramref name="producer"/> over and over until it returns a value.
        /// </summary>
        public static TResult Retry<TResult>(Func<Option<TResult>> producer)
            where TResult : notnull
            => producer().GetOrElse(() => Retry(producer));

        /// <summary>
        /// Calls the given <paramref name="producer"/> repeatedly until it returns a value or the retry policy conditions are no longer met.
        /// </summary>
        public static Option<TResult> Retry<TResult>(Func<Option<TResult>> producer, IRetryPolicy retryPolicy)
            where TResult : notnull
            => Enumerable
                .Repeat(producer(), FirstTry)
                .Concat(TailRetries(producer, retryPolicy))
                .FirstOrDefault(IsSome);

        public static async Task<Option<TResult>> RetryAsync<TResult>(Func<Task<Option<TResult>>> producer, IRetryPolicy retryPolicy)
            where TResult : notnull
            => await Enumerable
                .Repeat(producer(), FirstTry)
                .Concat(TailRetriesAsync(producer, retryPolicy))
                .FirstOrDefault();

        private static IEnumerable<Option<TResult>> TailRetries<TResult>(Func<Option<TResult>> producer, IRetryPolicy retryPolicy)
            where TResult : notnull
            => Retries(retryPolicy).Select(ProduceDelayed(producer, retryPolicy));

        private static IEnumerable<Task<Option<TResult>>> TailRetriesAsync<TResult>(Func<Task<Option<TResult>>> producer, IRetryPolicy retryPolicy)
            where TResult : notnull
            => Retries(retryPolicy).Select(ProduceDelayedAsync(producer, retryPolicy));

        private static IEnumerable<int> Retries(IRetryPolicy retryPolicy) => Enumerable.Range(0, retryPolicy.MaxRetries);

        private static bool IsSome<TResult>(Option<TResult> option)
            where TResult : notnull
            => option.Match(none: false, some: True);

        private static Func<int, Option<TResult>> ProduceDelayed<TResult>(Func<Option<TResult>> producer, IRetryPolicy retryPolicy)
            where TResult : notnull
            => retryCount =>
            {
                Sleep(retryPolicy.Duration(retryCount));

                return producer();
            };

        private static Func<int, Task<Option<TResult>>> ProduceDelayedAsync<TResult>(Func<Task<Option<TResult>>> producer, IRetryPolicy retryPolicy)
            where TResult : notnull
            => async retryCount =>
            {
                await Task.Delay(retryPolicy.Duration(retryCount));
                return await producer();
            };

        private static async Task<Option<TItem>> FirstOrDefault<TItem>(this IEnumerable<Task<Option<TItem>>> enumerable)
            where TItem : notnull
        {
            foreach (var item in enumerable)
            {
                var awaitedItem = await item;
                if (IsSome(awaitedItem))
                {
                    return awaitedItem;
                }
            }

            return Option<TItem>.None();
        }
    }
}
