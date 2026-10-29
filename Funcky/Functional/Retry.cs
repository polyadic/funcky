using Funcky.RetryPolicies;
using static System.Threading.Thread;

namespace Funcky;

public static partial class Functional
{
    /// <summary>
    /// Calls the given <paramref name="producer"/> over and over until it returns a value.
    /// </summary>
    /// <remarks>
    /// This overload never gives up and never waits between attempts. Use the overload with an <see cref="IRetryPolicy"/>
    /// when the producer can fail permanently or should not be called in a tight loop.
    /// </remarks>
    public static TResult Retry<TResult>(Func<Option<TResult>> producer)
        where TResult : notnull
        => Enumerable
            .Cycle(producer)
            .Select(produce => produce())
            .WhereSelect()
            .First();

    /// <summary>
    /// Calls the given <paramref name="producer"/> repeatedly until it returns a value or the retry policy conditions are no longer met.
    /// </summary>
    /// <remarks>Note that this function blocks the calling thread with <see cref="Thread.Sleep(TimeSpan)"/> for the delay between retries.
    /// Consider using <c>AsyncFunctional.RetryAsync</c> instead (part of Funcky on .NET 10 and newer, part of the <c>Funcky.Async</c> package on older frameworks).</remarks>
    public static Option<TResult> Retry<TResult>(Func<Option<TResult>> producer, IRetryPolicy retryPolicy)
        where TResult : notnull
        => Enumerable
            .Return(producer())
            .Concat(TailRetries(producer, retryPolicy))
            .WhereSelect()
            .FirstOrNone();

    private static IEnumerable<Option<TResult>> TailRetries<TResult>(Func<Option<TResult>> producer, IRetryPolicy retryPolicy)
        where TResult : notnull
        => Retries(retryPolicy)
            .Select(ProduceDelayed(producer, retryPolicy));

    private static IEnumerable<int> Retries(IRetryPolicy retryPolicy)
        => Enumerable.Range(1, retryPolicy.MaxRetries);

    private static Func<int, Option<TResult>> ProduceDelayed<TResult>(Func<Option<TResult>> producer, IRetryPolicy retryPolicy)
        where TResult : notnull
        => retryCount
            =>
            {
                Sleep(retryPolicy.Delay(retryCount));
                return producer();
            };
}
