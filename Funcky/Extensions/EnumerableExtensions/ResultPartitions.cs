namespace Funcky.Extensions;

public readonly struct ResultPartitions<TValidResult>
{
    public ResultPartitions(IReadOnlyCollection<TValidResult> ok, IReadOnlyCollection<Exception> error) => (Ok, Error) = (ok, error);

    public IReadOnlyCollection<TValidResult> Ok { get; }

    public IReadOnlyCollection<Exception> Error { get; }

    public void Deconstruct(out IReadOnlyCollection<TValidResult> ok, out IReadOnlyCollection<Exception> error) => (ok, error) = (Ok, Error);
}
