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

/// <remarks>
/// Result values constructed using <c>default</c> are in an invalid state.
/// Any attempt to perform actions on such a value will throw a <see cref="NotSupportedException"/>.
/// </remarks>
[NonDefaultable]
public readonly partial struct Result<TValidResult> : IEquatable<Result<TValidResult>>
    where TValidResult : notnull
{
    #if !SET_CURRENT_STACK_TRACE_SUPPORTED
    private const int SkipLowestStackFrame = 1;
    #endif

    private readonly TValidResult _result;
    private readonly Exception? _error;
    private readonly State _state;

    internal Result(TValidResult result)
    {
        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        _result = result;
        _state = State.Ok;
    }

    private Result(Exception error)
    {
        _result = default!;
        _error = error;
        _state = State.Error;
    }

    private enum State : byte
    {
        Uninitialized,
        Ok,
        Error,
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
    public TMatchResult Match<TMatchResult>(Func<Exception, TMatchResult> error, Func<TValidResult, TMatchResult> ok)
        => _state switch
        {
            State.Ok => ok(_result),
            State.Error => error(_error!),
            State.Uninitialized => throw UninitializedException(),
            _ => throw UnreachableException(),
        };

    [UseWithArgumentNames]
    public void Switch(Action<Exception> error, Action<TValidResult> ok)
    {
        switch (_state)
        {
            case State.Ok:
                ok(_result);
                break;
            case State.Error:
                error(_error!);
                break;
            case State.Uninitialized:
                throw UninitializedException();
            default:
                throw UnreachableException();
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
            uninitialized: static () => 0,
            error: static error => error.GetHashCode(),
            ok: static result => result.GetHashCode());

    [Pure]
    public override string ToString()
        => Match(
            uninitialized: static () => "Uninitialized",
            error: static exception => $"Error({exception.GetType().FullName}: {exception.Message})",
            ok: static result => $"Ok({result})");

    /// <summary>A total match that also handles the invalid state of a <c>default</c> value; for members that must never throw.</summary>
    [Pure]
    [UseWithArgumentNames]
    internal TMatchResult Match<TMatchResult>(Func<TMatchResult> uninitialized, Func<Exception, TMatchResult> error, Func<TValidResult, TMatchResult> ok)
        => _state switch
        {
            State.Ok => ok(_result),
            State.Error => error(_error!),
            State.Uninitialized => uninitialized(),
            _ => throw UnreachableException(),
        };

    private static NotSupportedException UninitializedException()
        => new($"Result constructed via default instead of a factory function ({nameof(Result)}.{nameof(Result.Ok)} or {nameof(Result<TValidResult>)}.{nameof(Error)})");

    private NotSupportedException UnreachableException()
        => new($"Unreachable: Enum variant {_state} is not handled");
}

public static partial class Result
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
