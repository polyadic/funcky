namespace Funcky.Collections;

public sealed class RedNode<TItem>
{
    private readonly GreenNode<TItem> _value;

    public RedNode(GreenNode<TItem> value, RedNode<TItem>? parent)
    {
        _value = value;
        Parent = parent;
        Children = value.Children.Select(ToRedNode).ToList();
    }

    public TItem Value
        => _value.Value;

    public IReadOnlyList<RedNode<TItem>> Children { get; }

    public RedNode<TItem>? Parent { get; }

    private RedNode<TItem> ToRedNode(GreenNode<TItem> node)
        => new(node, this);
}
