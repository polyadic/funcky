using System;
using System.Collections.Generic;
using System.Linq;
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

        private static IEnumerable<Option<TResult>> TailRetries<TResult>(Func<Option<TResult>> producer, IRetryPolicy retryPolicy)
            where TResult : notnull
            => Enumerable
                .Range(0, retryPolicy.MaxRetries)
                .Select(ProduceDelayed(producer, retryPolicy));

        private static bool IsSome<TResult>(Option<TResult> option)
            where TResult : notnull
            => option.Match(none: false, some: True);

        private static Func<int, Option<TResult>> ProduceDelayed<TResult>(Func<Option<TResult>> producer, IRetryPolicy retryPolicy)
            where TResult : notnull
            => (retryCount) =>
            {
                // We do not wait before the first try!
                Sleep(retryPolicy.Duration(retryCount));

                return producer();
            };
    }
}
