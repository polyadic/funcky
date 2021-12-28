namespace Collections;

public sealed class GreenNode<TItem>
{
    public GreenNode(TItem value, IEnumerable<GreenNode<TItem>> children)
    {
        Value = value;
        Children = children;
    }

    public TItem Value { get; }

    public IEnumerable<GreenNode<TItem>> Children { get; }
}
