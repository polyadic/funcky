namespace Funcky.Extensions;

public readonly struct Partitions<TSource>
{
    public Partitions(IReadOnlyList<TSource> @true, IReadOnlyList<TSource> @false) => (True, False) = (@true, @false);

    public IReadOnlyList<TSource> True { get; }

    public IReadOnlyList<TSource> False { get; }

    public void Deconstruct(out IReadOnlyList<TSource> @true, out IReadOnlyList<TSource> @false) => (@true, @false) = (True, False);
}

public static class Partitions
{
    public static Partitions<TSource> Create<TSource>(IReadOnlyList<TSource> @true, IReadOnlyList<TSource> @false)
        => new(@true, @false);
}
