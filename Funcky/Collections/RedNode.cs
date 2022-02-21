namespace Funcky.Collections;

public sealed class RedNode<TItem>
{
    private readonly GreenNode<TItem> _value;

    public RedNode(GreenNode<TItem> value, RedNode<TItem>? parent)
        => (_value, Parent) = (value, parent);

    public TItem Value
        => _value.Value;

    public IEnumerable<RedNode<TItem>> Children
        => _value.Children.Select(ToRedNode);

    public RedNode<TItem>? Parent { get; }

    private RedNode<TItem> ToRedNode(GreenNode<TItem> node)
        => new(node, this);
}
