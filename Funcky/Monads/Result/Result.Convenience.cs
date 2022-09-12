using System.Runtime.ExceptionServices;

namespace Funcky.Monads;

public readonly partial struct Result<TValidResult>
{
    public static implicit operator Result<TValidResult>(TValidResult item) => Result.Ok(item);

    /// <summary>Performs a side effect when the result is ok and returns the result again.</summary>
    public Result<TValidResult> Inspect(Action<TValidResult> inspector)
    {
        Switch(ok: inspector, error: NoOperation);
        return this;
    }

    public TValidResult GetOrThrow()
        => Match(
            ok: Identity,
            error: ThrowWithOriginalStackTrace);

    private static TValidResult ThrowWithOriginalStackTrace(Exception exception)
    {
        ExceptionDispatchInfo.Capture(exception).Throw();

        throw new Exception("unreachable", exception);
    }
}
