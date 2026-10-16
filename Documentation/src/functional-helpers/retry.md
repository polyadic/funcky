# Retry

Repeats an operation until it succeeds, or until a retry policy says to stop.

There are two families. The first works with an `Option`-returning producer, where `None` means "not yet":

```cs
TResult Retry<TResult>(Func<Option<TResult>> producer)
Option<TResult> Retry<TResult>(Func<Option<TResult>> producer, IRetryPolicy retryPolicy)
```

The second works with a producer that throws, and a predicate deciding which exceptions are worth retrying:

```cs
TResult Retry<TResult>(Func<TResult> producer, Func<Exception, bool> shouldRetry, IRetryPolicy retryPolicy)
void Retry(Action action, Func<Exception, bool> shouldRetry, IRetryPolicy retryPolicy)
```

## The Option-based form

A producer that returns `Option<T>` is the functional way to say "this may not have worked": no exception,
no `bool` and `out` parameter, just a value that is either there or not. `Retry` calls it again until it is.

The overload without a policy retries forever, with no delay, and returns the value directly, because it only
returns once there is one. Use it only when success is guaranteed eventually, for example polling an in-memory
state that another thread will set.

The overload with a policy gives up after the policy's `MaxRetries`, sleeping between attempts for the delay the
policy computes, and returns `None` if all attempts failed. The producer is called once immediately and then
once per retry, so at most `MaxRetries + 1` times.

```cs
Option<Response> response = Retry(
    () => TrySendRequest(),                              // Func<Option<Response>>
    new ExponentialBackOffRetryPolicy(maxRetries: 5, firstDelay: TimeSpan.FromMilliseconds(200)));
```

## The exception-based form

Most APIs signal transient failure by throwing. This form wraps such a call: if the producer throws and
`shouldRetry` returns `true` for the exception, it sleeps for the policy's delay and tries again, up to
`MaxRetries` times. An exception that `shouldRetry` rejects, or the last retry's exception, propagates to the
caller unchanged.

```cs
var content = Retry(
    () => httpClient.GetStringAsync(url).Result,
    shouldRetry: e => e is HttpRequestException,
    retryPolicy: new LinearBackOffRetryPolicy(maxRetries: 3, firstDelay: TimeSpan.FromSeconds(1)));
```

Both forms block the thread with `Thread.Sleep` during delays. In asynchronous code use `RetryAsync` and
`RetryAwaitAsync` from `AsyncFunctional`, which await the delay, accept a `CancellationToken`, and have
producers returning `ValueTask`. They live in `Funcky.Async`, and in `Funcky` itself on .NET 10.

The async forms check the token before every attempt and never retry once it is cancelled, even when
`shouldRetry` would accept the exception. To abort an attempt that is already in progress, use the overloads
whose producer receives the token:

```cs
var content = await RetryAwaitAsync(
    token => httpClient.GetStringAsync(url, token),
    shouldRetry: e => e is HttpRequestException,
    retryPolicy: new LinearBackOffRetryPolicy(maxRetries: 3, firstDelay: TimeSpan.FromSeconds(1)),
    cancellationToken);
```

## Retry policies

A policy is an `IRetryPolicy` with two members: how many retries to make, and how long to wait before each one.

```cs
public interface IRetryPolicy
{
    int MaxRetries { get; }
    TimeSpan Delay(int retryCount);
}
```

`Delay` receives the number of the retry about to be made, starting at 1, and returns the time to wait before
it. The built-in policies:

| Policy                                              | Delay before retry *n*                      |
|-----------------------------------------------------|---------------------------------------------|
| `NoDelayRetryPolicy(maxRetries)`                    | none                                        |
| `ConstantDelayPolicy(maxRetries, delay)`            | `delay`                                     |
| `LinearBackOffRetryPolicy(maxRetries, firstDelay)`  | `firstDelay × n`                            |
| `ExponentialBackOffRetryPolicy(maxRetries, firstDelay)` | `firstDelay × 1.5ⁿ⁻¹`, so the first retry waits `firstDelay` |
| `DoNotRetryPolicy()`                                | no retries at all                           |

`DoNotRetryPolicy` is useful as a default or in tests, where the retry wrapper should be transparent.
Implement the interface yourself for jitter, a cap on the delay, or a delay that depends on external state.
