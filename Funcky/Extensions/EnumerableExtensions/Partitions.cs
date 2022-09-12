namespace Funcky.Extensions;

public static class Partitions
{
    public static Partitions<TItem> Create<TItem>(IReadOnlyList<TItem> @true, IReadOnlyList<TItem> @false)
        => new(@true, @false);
}

public readonly struct Partitions<TItem>
{
    public Partitions(IReadOnlyList<TItem> @true, IReadOnlyList<TItem> @false) => (True, False) = (@true, @false);

    public IReadOnlyList<TItem> True { get; }

    public IReadOnlyList<TItem> False { get; }

    public void Deconstruct(out IReadOnlyList<TItem> @true, out IReadOnlyList<TItem> @false) => (@true, @false) = (True, False);
}
