using Funcky.RetryPolicies;
using static System.Threading.Thread;

namespace Funcky;

public static partial class Functional
{
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
        => Sequence
            .Return(producer())
            .Concat(TailRetries(producer, retryPolicy))
            .WhereSelect()
            .FirstOrNone();

    private static IEnumerable<Option<TResult>> TailRetries<TResult>(Func<Option<TResult>> producer, IRetryPolicy retryPolicy)
        where TResult : notnull
        => Retries(retryPolicy)
            .Select(ProduceDelayed(producer, retryPolicy));

    private static IEnumerable<int> Retries(IRetryPolicy retryPolicy)
        => Enumerable.Range(0, retryPolicy.MaxRetries);

    private static Func<int, Option<TResult>> ProduceDelayed<TResult>(Func<Option<TResult>> producer, IRetryPolicy retryPolicy)
        where TResult : notnull
        => retryCount
            =>
            {
                Sleep(retryPolicy.Duration(retryCount));
                return producer();
            };
}
