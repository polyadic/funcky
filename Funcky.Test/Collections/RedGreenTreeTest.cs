using Funcky.Collections;

namespace Funcky.Test.Collections
{
    public sealed class RedGreenTreeTest
    {
        [Fact]
        public void ChangingTheGreenTreeDoesNotInfluenceTheRedTree()
        {
            var greenRoot = new GreenNode<int>(100);
            greenRoot.Children.Add(new GreenNode<int>(13));
            greenRoot.Children.Add(new GreenNode<int>(37));

            var tree = new RedGreenTree<int>(greenRoot);

            var redRoot = tree.RootNode;

            greenRoot.Children.Clear();
            greenRoot.Children.Add(new GreenNode<int>(-1));

            // the red tree has not changed
            Assert.Equal(150, Sum(redRoot));

            // but we can materialize a new tree
            Assert.Equal(99, Sum(tree.RootNode));
        }

        private static int Sum(RedNode<int> redNode)
            => redNode.Value + redNode.Children.Aggregate(0, (sum, node) => sum + Sum(node));
    }
}
