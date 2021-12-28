namespace Collections
{
    public sealed class RedGreenTree<TItem>
    {
        private readonly GreenNode<TItem> _rootNode;

        public RedGreenTree(GreenNode<TItem> rootNode)
            => _rootNode = rootNode;

        public RedNode<TItem> RootNode
            => new(_rootNode, null);
    }
}
