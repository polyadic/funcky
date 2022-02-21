namespace Funcky.Collections
{
    public sealed class RedGreenTree<TItem>
    {
        private readonly GreenNode<TItem> _rootNode;

        public RedGreenTree(GreenNode<TItem> rootNode)
            => _rootNode = rootNode;

        public RedNode<TItem> RootNode
        {
            get
            {
                var root = new RedNode<TItem>(_rootNode, null);

                root.Children.Materialize();

                return root;
            }
        }
    }
}
