namespace Funcky.Extensions;

public static class Partitions
{
    public static Partitions<TItem> Create<TItem>(IReadOnlyList<TItem> trues, IReadOnlyList<TItem> falses)
        => new(trues, falses);
}

public readonly struct Partitions<TItem>
{
    public Partitions(IReadOnlyList<TItem> trues, IReadOnlyList<TItem> falses) => (True, False) = (trues, falses);

    public IReadOnlyList<TItem> True { get; }

    public IReadOnlyList<TItem> False { get; }

    public void Deconstruct(out IReadOnlyList<TItem> trues, out IReadOnlyList<TItem> falses) => (trues, falses) = (True, False);
}
