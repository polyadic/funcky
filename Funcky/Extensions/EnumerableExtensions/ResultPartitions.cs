namespace Funcky.Extensions;

public readonly struct ResultPartitions<TValidResult>
{
    public ResultPartitions(IReadOnlyCollection<Exception> error, IReadOnlyCollection<TValidResult> ok) => (Error, Ok) = (error, ok);

    public IReadOnlyCollection<Exception> Error { get; }

    public IReadOnlyCollection<TValidResult> Ok { get; }

    public void Deconstruct(out IReadOnlyCollection<Exception> error, out IReadOnlyCollection<TValidResult> ok) => (error, ok) = (Error, Ok);
}
