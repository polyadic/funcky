using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Funcky.Spike.Unions;

/// <summary>
/// Shape 3 for <c>Result&lt;T&gt;</c>: today's layout with <c>[Union]</c> and <c>IUnion</c>. The cases are
/// <c>T</c> (ok) and <see cref="Exception"/> (error): <c>result is Exception e</c> and <c>result is T value</c>.
/// <c>default</c> reports a <c>null</c> Value (CS8655 at the switch, NotSupportedException in Match, as today).
/// <para>Probe: the union members come from a nested <c>IUnionMembers</c> provider interface (spec: "union member
/// provider"), so the constructors stay private and <c>Value</c> is an explicit interface implementation that does
/// not show up on the struct.</para>
/// </summary>
[Union]
public readonly struct CustomUnionResult<T> : CustomUnionResult<T>.IUnionMembers, IUnion, IEquatable<CustomUnionResult<T>>
    where T : notnull
{
    private readonly T _result;
    private readonly Exception? _error;
    private readonly State _state;

    private CustomUnionResult(T result)
    {
        ArgumentNullException.ThrowIfNull(result);
        _result = result;
        _state = State.Ok;
    }

    private CustomUnionResult(Exception error)
    {
        ArgumentNullException.ThrowIfNull(error);
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

    /// <summary>Union member provider: the cases are the parameter types of its static <c>Create</c> methods.</summary>
    public interface IUnionMembers
    {
        public static CustomUnionResult<T> Create(T value) => new(value);

        public static CustomUnionResult<T> Create(Exception error) => new(error);

        public object? Value { get; }

        public bool HasValue { get; }

        public bool TryGetValue([NotNullWhen(true)] out T? result);

        public bool TryGetValue([NotNullWhen(true)] out Exception? error);
    }

    /// <summary>Non-boxing access pattern: the compiler uses these instead of <c>Value</c> for type patterns.</summary>
    public bool HasValue => _state != State.Uninitialized;

    object? IUnionMembers.Value => _state switch
    {
        State.Ok => _result,
        State.Error => _error,
        _ => null,
    };

    object? IUnion.Value => ((IUnionMembers)this).Value;

    public static implicit operator CustomUnionResult<T>(T result) => new(result);

    public static CustomUnionResult<T> Ok(T result) => new(result);

    public static CustomUnionResult<T> Error(Exception error) => new(error);

    public bool TryGetValue([NotNullWhen(true)] out T? result)
    {
        result = _state == State.Ok ? _result : default;
        return _state == State.Ok;
    }

    public bool TryGetValue([NotNullWhen(true)] out Exception? error)
    {
        error = _error;
        return _state == State.Error;
    }

    public TResult Match<TResult>(Func<Exception, TResult> error, Func<T, TResult> ok)
        => _state switch
        {
            State.Ok => ok(_result),
            State.Error => error(_error!),
            _ => throw new NotSupportedException("Result constructed via default is not supported."),
        };

    public CustomUnionResult<TResult> Select<TResult>(Func<T, TResult> selector)
        where TResult : notnull
        => _state switch
        {
            State.Ok => new CustomUnionResult<TResult>(selector(_result)),
            State.Error => new CustomUnionResult<TResult>(_error!),
            _ => throw new NotSupportedException("Result constructed via default is not supported."),
        };

    public bool Equals(CustomUnionResult<T> other)
        => _state == other._state
            && EqualityComparer<T>.Default.Equals(_result, other._result)
            && Equals(_error, other._error);

    public override bool Equals(object? obj) => obj is CustomUnionResult<T> other && Equals(other);

    public override int GetHashCode() => _state switch
    {
        State.Ok => _result.GetHashCode(),
        State.Error => _error!.GetHashCode(),
        _ => 0,
    };

    public override string ToString() => _state switch
    {
        State.Ok => $"Ok({_result})",
        State.Error => $"Error({_error!.GetType().Name})",
        _ => "Uninitialized",
    };
}
