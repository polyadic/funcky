using System;
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
                .Range(0, FirstTry + retryPolicy.MaxRetries)
                .Select(ProduceDelayed(producer, retryPolicy))
                .FirstOrDefault(IsSome<TResult>);

        private static bool IsSome<TResult>(Option<TResult> option)
            where TResult : notnull
            => option.Match(none: false, some: True);

        private static Func<int, Option<TResult>> ProduceDelayed<TResult>(Func<Option<TResult>> producer, IRetryPolicy retryPolicy)
            where TResult : notnull
            => (retryCount) =>
            {
                // We do not wait before the first try!
                if (IsNotFirstTry(retryCount))
                {
                    Sleep(retryPolicy.Duration(retryCount));
                }

                return producer();
            };

        private static bool IsNotFirstTry(int retryCount)
            => retryCount != 0;
    }
}
