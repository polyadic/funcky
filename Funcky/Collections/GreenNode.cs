namespace Funcky.Collections;

public sealed class GreenNode<TItem>
{
    public GreenNode(TItem value)
        => Value = value;

    public TItem Value { get; }

    public List<GreenNode<TItem>> Children { get; } = new();
}
