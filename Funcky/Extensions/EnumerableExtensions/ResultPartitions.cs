namespace Funcky.Extensions;

public readonly struct ResultPartitions<TValidResult>
{
    public ResultPartitions(IReadOnlyList<Exception> error, IReadOnlyList<TValidResult> ok) => (Error, Ok) = (error, ok);

    public IReadOnlyList<Exception> Error { get; }

    public IReadOnlyList<TValidResult> Ok { get; }

    public void Deconstruct(out IReadOnlyList<Exception> error, out IReadOnlyList<TValidResult> ok) => (error, ok) = (Error, Ok);
}

public static class ResultPartitions
{
    public static ResultPartitions<TValidResult> Create<TValidResult>(IReadOnlyList<Exception> error, IReadOnlyList<TValidResult> ok)
        => new(error, ok);
}
