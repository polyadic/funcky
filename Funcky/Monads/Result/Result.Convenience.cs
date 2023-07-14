using System.Runtime.ExceptionServices;

namespace Funcky.Monads;

public readonly partial struct Result<TValidResult>
{
    public static implicit operator Result<TValidResult>(TValidResult result) => Result.Ok(result);

    /// <summary>Performs a side effect when the result is ok and returns the result again.</summary>
    public Result<TValidResult> Inspect(Action<TValidResult> inspector)
    {
        Switch(ok: inspector, error: NoOperation);
        return this;
    }

    /// <remarks>Careful! This overload discards the exception.</remarks>
    [Pure]
    public Result<TValidResult> OrElse(Result<TValidResult> fallback)
        => Match(error: _ => fallback, ok: Result.Return);

    [Pure]
    public Result<TValidResult> OrElse(Func<Exception, Result<TValidResult>> fallback)
        => Match(error: fallback, ok: Result.Return);

    /// <remarks>Careful! This overload discards the exception.</remarks>
    [Pure]
    public TValidResult GetOrElse(TValidResult fallback)
        => Match(error: _ => fallback, ok: Identity);

    [Pure]
    public TValidResult GetOrElse(Func<Exception, TValidResult> fallback)
        => Match(error: fallback, ok: Identity);

    public TValidResult GetOrThrow()
        => GetOrElse(ThrowWithOriginalStackTrace);

    private static TValidResult ThrowWithOriginalStackTrace(Exception exception)
    {
        ExceptionDispatchInfo.Capture(exception).Throw();

        throw new Exception("unreachable", exception);
    }
}
