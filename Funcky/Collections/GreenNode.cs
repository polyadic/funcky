namespace Funcky.Collections;

public sealed class GreenNode<TItem>
{
    public GreenNode(TItem value, IEnumerable<GreenNode<TItem>> children)
        => (Value, Children) = (value, children);

    public TItem Value { get; }

    public IEnumerable<GreenNode<TItem>> Children { get; }
}
