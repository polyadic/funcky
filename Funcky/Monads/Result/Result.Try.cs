using Funcky.Internal;

namespace Funcky.Monads;

public static partial class Result
{
    /// <summary>Runs the function and captures its outcome: <c>Ok(value)</c> when it returns, <c>Error(exception)</c> when it throws.</summary>
    /// <remarks>Exceptions that signal a corrupted process (<see cref="OutOfMemoryException"/>, <see cref="StackOverflowException"/>,
    /// <see cref="AccessViolationException"/>, <see cref="System.Threading.ThreadAbortException"/>) are not captured and propagate.
    /// Neither is <see cref="OperationCanceledException"/>: a cancelled operation has no result, not an error result.</remarks>
    /// <typeparam name="TValidResult">Type of the value the function returns.</typeparam>
    /// <param name="function">The function to run.</param>
    /// <example><code>
    /// Result&lt;int&gt; parsed = Result.Try(() =&gt; int.Parse(text));
    /// </code></example>
    public static Result<TValidResult> Try<TValidResult>(Func<TValidResult> function)
        where TValidResult : notnull
    {
        try
        {
            return Ok(function());
        }
        catch (Exception exception) when (!exception.IsFatal && exception is not OperationCanceledException)
        {
            return Result<TValidResult>.Error(exception);
        }
    }

    /// <summary>Runs the asynchronous function and captures its outcome: <c>Ok(value)</c> when the task completes,
    /// <c>Error(exception)</c> when the function or its task throws.</summary>
    /// <remarks>Exceptions that signal a corrupted process (<see cref="OutOfMemoryException"/>, <see cref="StackOverflowException"/>,
    /// <see cref="AccessViolationException"/>, <see cref="System.Threading.ThreadAbortException"/>) are not captured and propagate.
    /// Neither is <see cref="OperationCanceledException"/>: a cancelled operation has no result, not an error result.</remarks>
    /// <typeparam name="TValidResult">Type of the value the task produces.</typeparam>
    /// <param name="function">The function to run.</param>
    /// <example><code>
    /// Result&lt;string&gt; body = await Result.TryAsync(() =&gt; client.GetStringAsync(uri));
    /// </code></example>
    public static async Task<Result<TValidResult>> TryAsync<TValidResult>(Func<Task<TValidResult>> function)
        where TValidResult : notnull
    {
        try
        {
            return Ok(await function().ConfigureAwait(false));
        }
        catch (Exception exception) when (!exception.IsFatal && exception is not OperationCanceledException)
        {
            return Result<TValidResult>.Error(exception);
        }
    }
}
