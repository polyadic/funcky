namespace Funcky.Spike.Unions;

/// <summary>
/// <c>Result&lt;T&gt;</c> as a union with <see cref="Exception"/> itself as the error case: no wrapper, and the pattern
/// <c>result is Exception e</c> reads naturally. The error case is a class, so the union stores it without boxing;
/// only the <c>Ok</c> struct boxes.
/// </summary>
public partial union UnionResult<T>(Ok<T>, Exception)
    where T : notnull
{
    public static implicit operator UnionResult<T>(T value) => new Ok<T>(value);

    public TResult Match<TResult>(Func<Exception, TResult> error, Func<T, TResult> ok)
        => this switch
        {
            Ok<T>(var value) => ok(value),
            Exception exception => error(exception),
            _ => throw new NotSupportedException("Result constructed via default is not supported."),
        };

    public UnionResult<TResult> Select<TResult>(Func<T, TResult> selector)
        where TResult : notnull
        => this switch
        {
            Ok<T>(var value) => new Ok<TResult>(selector(value)),
            Exception exception => new UnionResult<TResult>(exception),
            _ => throw new NotSupportedException("Result constructed via default is not supported."),
        };

    public override string ToString()
        => this switch
        {
            Ok<T>(var value) => $"Ok({value})",
            Exception exception => $"Error({exception.GetType().Name})",
            _ => "Uninitialized",
        };
}
