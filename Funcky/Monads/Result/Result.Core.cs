using Funcky.CodeAnalysis;
#if SET_CURRENT_STACK_TRACE_SUPPORTED
using System.Runtime.ExceptionServices;
#if STACK_TRACE_HIDDEN_SUPPORTED
using System.Diagnostics;
#else
using System.Runtime.CompilerServices;
#endif
#else
using System.Diagnostics;
#endif

namespace Funcky.Monads;

public readonly partial struct Result<TValidResult> : IEquatable<Result<TValidResult>>
    where TValidResult : notnull
{
    #if !SET_CURRENT_STACK_TRACE_SUPPORTED
    private const int SkipLowestStackFrame = 1;
    #endif

    private readonly TValidResult _result;
    private readonly Exception? _error;

    internal Result(TValidResult result)
    {
        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        _result = result;
    }

    private Result(Exception error)
    {
        _result = default!;
        _error = error;
    }

    [Pure]
    public static bool operator ==(Result<TValidResult> left, Result<TValidResult> right)
        => left.Equals(right);

    [Pure]
    public static bool operator !=(Result<TValidResult> left, Result<TValidResult> right)
        => !left.Equals(right);

    /// <summary>Creates a new <see cref="Result{TValidResult}"/> from an <see cref="Exception"/> and sets
    /// the stack trace if not already set.</summary>
    /// <remarks>This method has side effects: It sets the stack trace on <paramref name="exception"/> if not already set.</remarks>
    #if SET_CURRENT_STACK_TRACE_SUPPORTED
    // Methods with AggressiveInlining are always excluded from the stack trace.
    // This is required for <c>SetCurrentStackTrace</c> to work properly.
    // See: https://github.com/dotnet/runtime/blob/master/src/libraries/System.Private.CoreLib/src/System/Diagnostics/StackTrace.cs#L347
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [StackTraceHidden]
    #else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    #endif
    #endif
    public static Result<TValidResult> Error(Exception exception)
    {
        if (exception is null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        if (exception.StackTrace is null)
        {
            #if SET_CURRENT_STACK_TRACE_SUPPORTED
                ExceptionDispatchInfo.SetCurrentStackTrace(exception);
            #else
                exception.SetStackTrace(new StackTrace(SkipLowestStackFrame, true));
            #endif
        }

        return new Result<TValidResult>(exception);
    }

    [Pure]
    [UseWithArgumentNames]
    public TMatchResult Match<TMatchResult>(Func<TValidResult, TMatchResult> ok, Func<Exception, TMatchResult> error)
        => _error is null
            ? ok(_result)
            : error(_error);

    [UseWithArgumentNames]
    public void Switch(Action<TValidResult> ok, Action<Exception> error)
    {
        if (_error is null)
        {
            ok(_result);
        }
        else
        {
            error(_error);
        }
    }

    [Pure]
    public override bool Equals(object? obj)
        => obj is Result<TValidResult> other && Equals(other);

    [Pure]
    public bool Equals(Result<TValidResult> other)
        => Equals(_result, other._result)
           && Equals(_error, other._error);

    [Pure]
    public override int GetHashCode()
        => Match(
            ok: result => result?.GetHashCode(),
            error: error => error.GetHashCode()) ?? 0;

    [Pure]
    public override string ToString()
        => Match(
            ok: static result => $"Ok({result})",
            error: static exception => $"Error({exception.GetType().FullName}: {exception.Message})");
}

public static class Result
{
    [Pure]
    public static Result<TValidResult> Ok<TValidResult>(TValidResult result)
        where TValidResult : notnull
        => new(result);

    [Pure]
    public static Result<TValidResult> Return<TValidResult>(TValidResult result)
        where TValidResult : notnull
        => new(result);
}
